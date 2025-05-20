using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainDetector : MonoBehaviour
{
    // Reference to the tilemap that contains terrain information
    [SerializeField] private Tilemap terrainTilemap;
    
    // Dictionary mapping tile names to terrain types
    private Dictionary<string, AudioManager.TerrainType> tileToTerrainMapping = new Dictionary<string, AudioManager.TerrainType>();
    
    // Current terrain type the player is on
    private AudioManager.TerrainType currentTerrainType = AudioManager.TerrainType.Grass;

    private void Start()
    {
        // Initialize the tile to terrain mapping
        InitializeTileMapping();
        
        // If no tilemap is assigned, try to find one in the scene
        if (terrainTilemap == null)
        {
            terrainTilemap = FindObjectOfType<Tilemap>();
        }
    }

    private void Update()
    {
        // Detect the terrain type at the player's position
        DetectTerrain();
    }

    // Initialize the mapping between tile names and terrain types
    private void InitializeTileMapping()
    {
        // Add mappings for different tile names to terrain types
        // These should be customized based on your actual tile names
        tileToTerrainMapping.Add("grass", AudioManager.TerrainType.Grass);
        tileToTerrainMapping.Add("dirt", AudioManager.TerrainType.Dirt);
        tileToTerrainMapping.Add("gravel", AudioManager.TerrainType.Gravel);
        tileToTerrainMapping.Add("wood", AudioManager.TerrainType.Wood);
        tileToTerrainMapping.Add("stone", AudioManager.TerrainType.Stone);
        tileToTerrainMapping.Add("water", AudioManager.TerrainType.Water);
    }

    // Detect the terrain type at the player's position
    private void DetectTerrain()
    {
        if (terrainTilemap != null)
        {
            // Convert world position to cell position
            Vector3Int cellPosition = terrainTilemap.WorldToCell(transform.position);
            
            // Get the tile at the cell position
            TileBase tile = terrainTilemap.GetTile(cellPosition);
            
            if (tile != null)
            {
                // Try to determine terrain type based on tile name
                string tileName = tile.name.ToLower();
                
                foreach (var mapping in tileToTerrainMapping)
                {
                    if (tileName.Contains(mapping.Key))
                    {
                        SetTerrainType(mapping.Value);
                        return;
                    }
                }
            }
        }
        
        // Default to grass if no specific terrain is detected
        SetTerrainType(AudioManager.TerrainType.Grass);
    }

    // Set the current terrain type and update the AudioManager
    private void SetTerrainType(AudioManager.TerrainType terrainType)
    {
        if (terrainType != currentTerrainType)
        {
            currentTerrainType = terrainType;
            
            // Update the AudioManager with the new terrain type
            if (AudioManager.instance != null)
            {
                AudioManager.instance.SetTerrainType(currentTerrainType);
            }
        }
    }

    // Public method to get the current terrain type
    public AudioManager.TerrainType GetCurrentTerrainType()
    {
        return currentTerrainType;
    }
}
