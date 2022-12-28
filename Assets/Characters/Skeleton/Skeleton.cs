using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public float damage = 1;
    public float knockbackForce = 20f;
    public float moveSpeed = 500f;
    public CircleCollider2D triggerCollider;
    public IDamageable damageableObject;
    public Rigidbody2D playerRigidbody;
    
    public DetectionZone detectionZone;
    Rigidbody2D rb;
    
    DamageableCharacter damagableCharacter;

    // New field to represent the isMoving bool
    bool isMoving = false;

    void Start(){
        damageableObject = GetComponent<IDamageable>();
        rb = GetComponent<Rigidbody2D>();
        damagableCharacter = GetComponent<DamageableCharacter>();
        
        // Get the animator component and set the initial value of the isMoving parameter
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isMoving", isMoving);
    }

    void Update() {
        float currentHealth = damageableObject.Health;
        RaycastHit2D hit = Physics2D.Raycast(playerRigidbody.position, playerRigidbody.velocity, 0.5f);
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("GroundCollision")) {
            playerRigidbody.AddForce(-hit.normal * knockbackForce, ForceMode2D.Impulse);
        }
    if (currentHealth <= 0) {
        OnDeath();
    }
}
    

    void FixedUpdate() {
    if(damagableCharacter.Targetable && detectionZone.detectedObjs.Count > 0) {
        // Calculate direction to target object
        Vector2 direction = (detectionZone.detectedObjs[0].transform.position - transform.position).normalized;

        // Update the isMoving bool based on whether the slime is moving or not
        if(direction != Vector2.zero) {
            isMoving = true;

            // If the slime is moving to the right, set the x scale to be positive
            if(direction.x > 0) {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            // If the slime is moving to the left, set the x scale to be negative
            else if(direction.x < 0) {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        } else {
            isMoving = false;
        }

        // Move towards detected object
        rb.AddForce(direction * moveSpeed * Time.fixedDeltaTime);
    } else {
        // If there are no detected objects, the slime is not moving
        isMoving = false;
    }

    // Update the isMoving parameter in the animator based on the isMoving bool
    Animator animator = GetComponent<Animator>();
    animator.SetBool("isMoving", isMoving);
}
void OnDeath() {
    triggerCollider.enabled = false;
}


    /// Deal damage and knockback to IDamageable 
    void OnCollisionEnter2D(Collision2D collision) {
    // Get the animator component
    Animator animator = GetComponent<Animator>();

    // Check if the attack animation is currently playing
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    if (stateInfo.IsName("skeleton_attack")) {
        // Attack animation is playing, so apply damage and knockback to the IDamageable object

        Collider2D collider = collision.collider;
        IDamageable damageable = collider.GetComponent<IDamageable>();
        
        if(damageable != null) {
            // Offset for collision detection changes the direction where the force comes from
            Vector2 direction = (collider.transform.position - transform.position).normalized;

            // Knockback is in direction of swordCollider towards collider
            Vector2 knockback = direction * knockbackForce;

            // After making sure the collider has a script that implements IDamagable, we can run the OnHit implementation and pass our Vector2 force
            damageable.OnHit(damage, knockback);
        }
    }
}

}
