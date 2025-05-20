using UnityEngine;
using UnityEditor;
using System.IO;

public class AudioManagerSetup : EditorWindow
{
    [MenuItem("Tools/Audio/Setup Audio Manager")]
    public static void ShowWindow()
    {
        GetWindow<AudioManagerSetup>("Audio Manager Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Audio Manager Setup", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Audio Manager"))
        {
            CreateAudioManager();
        }
        
        if (GUILayout.Button("Add PlayerAudioHandler to Player"))
        {
            AddPlayerAudioHandler();
        }
        
        if (GUILayout.Button("Add TerrainDetector to Player"))
        {
            AddTerrainDetector();
        }
    }

    private void CreateAudioManager()
    {
        // Check if AudioManager already exists in the scene
        AudioManager existingManager = FindObjectOfType<AudioManager>();
        
        if (existingManager != null)
        {
            Debug.Log("AudioManager already exists in the scene.");
            Selection.activeGameObject = existingManager.gameObject;
            return;
        }
        
        // Create a new GameObject for the AudioManager
        GameObject audioManagerObj = new GameObject("AudioManager");
        AudioManager audioManager = audioManagerObj.AddComponent<AudioManager>();
        
        // Add audio sources
        AudioSource musicSource = audioManagerObj.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.volume = 0.5f;
        
        AudioSource footstepSource = audioManagerObj.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
        footstepSource.loop = false;
        footstepSource.volume = 0.3f;
        
        // Set the audio sources in the AudioManager
        SerializedObject serializedManager = new SerializedObject(audioManager);
        serializedManager.FindProperty("musicSource").objectReferenceValue = musicSource;
        serializedManager.FindProperty("footstepSource").objectReferenceValue = footstepSource;
        serializedManager.ApplyModifiedProperties();
        
        Debug.Log("AudioManager created successfully.");
        Selection.activeGameObject = audioManagerObj;
        
        // Create prefab if it doesn't exist
        string prefabPath = "Assets/Prefabs";
        string prefabName = "AudioManager.prefab";
        
        if (!Directory.Exists(prefabPath))
        {
            Directory.CreateDirectory(prefabPath);
            AssetDatabase.Refresh();
        }
        
        string fullPath = Path.Combine(prefabPath, prefabName);
        
        // Check if prefab already exists
        if (AssetDatabase.LoadAssetAtPath<GameObject>(fullPath) == null)
        {
            PrefabUtility.SaveAsPrefabAsset(audioManagerObj, fullPath);
            Debug.Log("AudioManager prefab created at: " + fullPath);
        }
    }

    private void AddPlayerAudioHandler()
    {
        // Find the player in the scene
        GameObject player = null;
        
        // Try to find through GameManager first
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.player != null)
        {
            player = gameManager.player.gameObject;
        }
        
        // If not found, try to find by tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // If still not found, try to find PlayerController
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.gameObject;
            }
        }
        
        if (player == null)
        {
            Debug.LogError("Could not find player in the scene!");
            return;
        }
        
        // Check if PlayerAudioHandler already exists
        PlayerAudioHandler existingHandler = player.GetComponent<PlayerAudioHandler>();
        
        if (existingHandler != null)
        {
            Debug.Log("PlayerAudioHandler already exists on the player.");
            Selection.activeGameObject = player;
            return;
        }
        
        // Add PlayerAudioHandler component
        PlayerAudioHandler handler = player.AddComponent<PlayerAudioHandler>();
        
        Debug.Log("PlayerAudioHandler added to player: " + player.name);
        Selection.activeGameObject = player;
    }

    private void AddTerrainDetector()
    {
        // Find the player in the scene
        GameObject player = null;
        
        // Try to find through GameManager first
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.player != null)
        {
            player = gameManager.player.gameObject;
        }
        
        // If not found, try to find by tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // If still not found, try to find PlayerController
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.gameObject;
            }
        }
        
        if (player == null)
        {
            Debug.LogError("Could not find player in the scene!");
            return;
        }
        
        // Check if TerrainDetector already exists
        TerrainDetector existingDetector = player.GetComponent<TerrainDetector>();
        
        if (existingDetector != null)
        {
            Debug.Log("TerrainDetector already exists on the player.");
            Selection.activeGameObject = player;
            return;
        }
        
        // Add TerrainDetector component
        TerrainDetector detector = player.AddComponent<TerrainDetector>();
        
        // Try to find a Tilemap in the scene
        UnityEngine.Tilemaps.Tilemap tilemap = FindObjectOfType<UnityEngine.Tilemaps.Tilemap>();
        
        if (tilemap != null)
        {
            // Set the tilemap in the TerrainDetector
            SerializedObject serializedDetector = new SerializedObject(detector);
            serializedDetector.FindProperty("terrainTilemap").objectReferenceValue = tilemap;
            serializedDetector.ApplyModifiedProperties();
        }
        
        Debug.Log("TerrainDetector added to player: " + player.name);
        Selection.activeGameObject = player;
    }
}
