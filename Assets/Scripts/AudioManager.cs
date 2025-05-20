using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager instance;

    [Header("Background Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private bool playMusicOnStart = true;

    // List to store all ambient music tracks
    private List<AudioClip> ambientMusicTracks = new List<AudioClip>();
    private int currentTrackIndex = 0;
    private bool isMusicPlaying = false;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private float footstepVolume = 0.5f;
    [SerializeField] private float minTimeBetweenFootsteps = 0.0f;
    [SerializeField] private float maxTimeBetweenFootsteps = 0.05f;

    // Dictionary to store footstep sounds by terrain type
    private Dictionary<TerrainType, List<AudioClip>> footstepSoundsByTerrain = new Dictionary<TerrainType, List<AudioClip>>();
    private float timeSinceLastFootstep = 0f;
    private float nextFootstepTime = 0.25f; // Initialize with a default value for faster walking pace
    private bool isPlayerMoving = false;
    private TerrainType currentTerrainType = TerrainType.Grass;

    // Enum for different terrain types (extensible for future additions)
    public enum TerrainType
    {
        Grass,
        Dirt,
        Gravel,
        Wood,
        Stone,
        Water
    }

    private void Awake()
    {
        // Singleton pattern implementation
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize audio sources if not set in inspector
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = false;
                musicSource.playOnAwake = false;
                musicSource.volume = musicVolume;
            }

            if (footstepSource == null)
            {
                footstepSource = gameObject.AddComponent<AudioSource>();
                footstepSource.loop = false;
                footstepSource.playOnAwake = false;
                footstepSource.volume = footstepVolume;
            }

            // Load all audio clips
            LoadAllAudioClips();

            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void Start()
    {
        if (playMusicOnStart)
        {
            PlayRandomMusicTrack();
        }
    }

    private void Update()
    {
        // Check if music has stopped playing and play the next track
        if (isMusicPlaying && !musicSource.isPlaying)
        {
            PlayNextMusicTrack();
        }

        // Handle footstep sounds
        if (isPlayerMoving)
        {
            timeSinceLastFootstep += Time.deltaTime;

            // Only try to play a new footstep sound if:
            // 1. Enough time has passed since the last footstep
            // 2. No footstep sound is currently playing
            if (timeSinceLastFootstep >= nextFootstepTime && !footstepSource.isPlaying)
            {
                PlayFootstepSound();
                timeSinceLastFootstep = 0f;

                // Calculate next footstep time - balanced for natural walking
                if (footstepSource.clip != null)
                {
                    // For a natural walking pace, we'll use a fraction of the clip length
                    // plus a small delay between steps
                    float clipLength = footstepSource.clip.length;

                    // This factor determines how much of the clip plays before the next step
                    // 0.4 means we wait for 40% of the clip to play before the next step (faster pace)
                    float walkingPaceFactor = 0.4f;

                    // Calculate the time to wait before the next footstep
                    nextFootstepTime = clipLength * walkingPaceFactor;

                    // Ensure the time is within reasonable bounds for faster walking
                    nextFootstepTime = Mathf.Clamp(nextFootstepTime, 0.2f, 0.35f);

                    // Add a tiny bit of randomness
                    nextFootstepTime += Random.Range(-0.05f, 0.05f);

                    // Debug log to verify footstep timing (uncomment for debugging)
                    // Debug.Log("Playing footstep sound. Next in: " + nextFootstepTime + " seconds");
                }
                else
                {
                    // Fallback if no clip is available
                    nextFootstepTime = 0.25f; // Faster default timing
                }
            }
        }
        else
        {
            // When player stops moving, reset the timer to play a step immediately when movement resumes
            // but only if we've waited long enough since the last step
            if (timeSinceLastFootstep >= nextFootstepTime)
            {
                timeSinceLastFootstep = nextFootstepTime;
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the player in the new scene
        FindAndConnectToPlayer();
    }

    // Load all audio clips from the respective folders
    private void LoadAllAudioClips()
    {
        // Load ambient music tracks
        LoadAmbientMusicTracks();

        // Load footstep sounds
        LoadFootstepSounds();
    }

    // Load all ambient music tracks from the specified folder
    private void LoadAmbientMusicTracks()
    {
        ambientMusicTracks.Clear();

        #if UNITY_EDITOR
        // In editor, we can use AssetDatabase to find and load assets
        string[] assetGuids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Sounds/Music/Ambient" });

        foreach (string guid in assetGuids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (clip != null)
            {
                ambientMusicTracks.Add(clip);
                Debug.Log("Loaded ambient music track: " + clip.name);
            }
        }
        #else
        // In a build, we need to use a different approach
        // This assumes you've set up a Resources folder with the audio clips
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/Music/Ambient");

        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                ambientMusicTracks.Add(clip);
                Debug.Log("Loaded ambient music track: " + clip.name);
            }
        }
        #endif

        if (ambientMusicTracks.Count == 0)
        {
            Debug.LogWarning("No ambient music tracks found! Make sure they are in Assets/Sounds/Music/Ambient or Resources/Sounds/Music/Ambient");
        }
    }

    // Load all footstep sounds for different terrain types
    private void LoadFootstepSounds()
    {
        // Initialize dictionary for each terrain type
        foreach (TerrainType terrainType in System.Enum.GetValues(typeof(TerrainType)))
        {
            footstepSoundsByTerrain[terrainType] = new List<AudioClip>();
        }

        // Load grass footstep sounds
        #if UNITY_EDITOR
        // In editor, we can use AssetDatabase to find and load assets
        string[] grassGuids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Sounds/Sounds/Walk/Grass" });

        foreach (string guid in grassGuids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (clip != null)
            {
                footstepSoundsByTerrain[TerrainType.Grass].Add(clip);
                Debug.Log("Loaded grass footstep sound: " + clip.name);
            }
        }

        // Load dirt footstep sounds if they exist
        string[] dirtGuids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Sounds/Sounds/Walk/Dirt" });

        foreach (string guid in dirtGuids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (clip != null)
            {
                footstepSoundsByTerrain[TerrainType.Dirt].Add(clip);
                Debug.Log("Loaded dirt footstep sound: " + clip.name);
            }
        }
        #else
        // In a build, we need to use a different approach
        // This assumes you've set up a Resources folder with the audio clips
        AudioClip[] grassClips = Resources.LoadAll<AudioClip>("Sounds/Sounds/Walk/Grass");

        foreach (AudioClip clip in grassClips)
        {
            if (clip != null)
            {
                footstepSoundsByTerrain[TerrainType.Grass].Add(clip);
                Debug.Log("Loaded grass footstep sound: " + clip.name);
            }
        }

        // Load dirt footstep sounds if they exist
        AudioClip[] dirtClips = Resources.LoadAll<AudioClip>("Sounds/Sounds/Walk/Dirt");

        foreach (AudioClip clip in dirtClips)
        {
            if (clip != null)
            {
                footstepSoundsByTerrain[TerrainType.Dirt].Add(clip);
                Debug.Log("Loaded dirt footstep sound: " + clip.name);
            }
        }
        #endif

        // Check if we have any footstep sounds
        if (footstepSoundsByTerrain[TerrainType.Grass].Count == 0)
        {
            Debug.LogWarning("No grass footstep sounds found! Make sure they are in Assets/Sounds/Sounds/Walk/Grass or Resources/Sounds/Sounds/Walk/Grass");
        }
    }

    // Play a random music track from the ambient music tracks
    public void PlayRandomMusicTrack()
    {
        if (ambientMusicTracks.Count > 0)
        {
            currentTrackIndex = Random.Range(0, ambientMusicTracks.Count);
            PlayMusicTrack(currentTrackIndex);
        }
    }

    // Play the next music track in sequence
    public void PlayNextMusicTrack()
    {
        if (ambientMusicTracks.Count > 0)
        {
            currentTrackIndex = (currentTrackIndex + 1) % ambientMusicTracks.Count;
            PlayMusicTrack(currentTrackIndex);
        }
    }

    // Play a specific music track by index
    public void PlayMusicTrack(int index)
    {
        if (ambientMusicTracks.Count > 0 && index >= 0 && index < ambientMusicTracks.Count)
        {
            musicSource.clip = ambientMusicTracks[index];
            musicSource.Play();
            isMusicPlaying = true;

            Debug.Log("Playing music track: " + ambientMusicTracks[index].name);
        }
    }

    // Play a footstep sound based on the current terrain type
    private void PlayFootstepSound()
    {
        // Don't play a new footstep sound if one is already playing
        if (footstepSource.isPlaying)
        {
            return;
        }

        List<AudioClip> footstepSounds = footstepSoundsByTerrain[currentTerrainType];

        if (footstepSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Count);
            AudioClip footstepClip = footstepSounds[randomIndex];

            // Vary the pitch slightly for more natural sounding footsteps
            // Higher pitch range makes footsteps sound faster
            float randomPitch = Random.Range(1.0f, 1.15f);
            footstepSource.pitch = randomPitch;

            footstepSource.clip = footstepClip;
            footstepSource.Play();

            // Debug log to verify sound is playing (uncomment for debugging)
            // Debug.Log("Playing footstep sound: " + footstepClip.name);
        }
        else
        {
            Debug.LogWarning("No footstep sounds available for terrain type: " + currentTerrainType);
        }
    }

    // Set the current terrain type for footstep sounds
    public void SetTerrainType(TerrainType terrainType)
    {
        currentTerrainType = terrainType;
    }

    // Set whether the player is moving (to be called from PlayerController)
    public void SetPlayerMoving(bool isMoving)
    {
        isPlayerMoving = isMoving;
    }

    // Play a single footstep sound for brief movements (like tapping a key once)
    public void PlaySingleFootstep()
    {
        // Only play if not already playing a sound
        if (!footstepSource.isPlaying)
        {
            PlayFootstepSound();
            // Reset the timer to prevent immediate playing of another step
            timeSinceLastFootstep = 0f;
        }
    }

    // Find the player in the scene and connect to its movement events
    private void FindAndConnectToPlayer()
    {
        // Try to find PlayerAudioHandler in the scene
        PlayerAudioHandler playerAudioHandler = FindObjectOfType<PlayerAudioHandler>();

        if (playerAudioHandler != null)
        {
            RegisterPlayer(playerAudioHandler);
            Debug.Log("Found and connected to player audio handler");
        }
        else
        {
            Debug.LogWarning("Could not find PlayerAudioHandler in the scene!");
        }
    }

    // Register a player audio handler with the audio manager
    public void RegisterPlayer(PlayerAudioHandler playerAudioHandler)
    {
        // Initial setup based on player's current state
        isPlayerMoving = playerAudioHandler.IsMoving();
        currentTerrainType = playerAudioHandler.GetCurrentTerrainType();

        Debug.Log("Registered player with AudioManager. Moving: " + isPlayerMoving + ", Terrain: " + currentTerrainType);
    }
}
