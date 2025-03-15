using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Random = UnityEngine.Random;
public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Color _pathColor = Color.white;
    [SerializeField] private Color _minigameTileColor = Color.yellow;
    [SerializeField] private Color _checkpointColor = Color.green;
    [SerializeField] private Color _eventTileColor = Color.cyan;

    [Header("Special Tile Settings")]
    [SerializeField] private float eventTileChance = 0.3f; // 30% chance for event tiles
    [SerializeField] private int minEventTiles = 5; // Minimum number of event tiles
    [SerializeField] private int minigameTilesFrequency = 15; // Place a minigame tile every ~15 tiles
    [SerializeField] private int checkpointFrequency = 25; // Place checkpoints every ~25 tiles

    // Tile dictionaries and lists
    private Dictionary<Vector2, Tile> _tiles = new Dictionary<Vector2, Tile>();
    private List<Tile> _pathTiles = new List<Tile>();

    // Distance threshold for position matching (for float comparison)
    [SerializeField] private float _positionThreshold = 0.1f;

    // Public property to get path length
    public int PathLength => _pathTiles.Count;

    // The hardcoded path positions
    private Vector2[] _pathPositions;

    private void Awake()
    {
        // Initialize the hardcoded path
        LoadHardcodedPath();
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void LoadHardcodedPath()
    {
        // Load the path from PathCode.txt
        _pathPositions = new Vector2[]
        {
    new Vector2(38.66f,-57.62647f),
    new Vector2(33.58334f,-57.62647f),
    new Vector2(28.50667f,-57.62647f),
    new Vector2(23.43f,-57.62647f),
    new Vector2(23.43f,-51.47647f),
    new Vector2(18.15f,-51.47247f),
    new Vector2(12.87f,-51.46847f),
    new Vector2(7.589999f,-51.46447f),
    new Vector2(2.309999f,-51.46047f),
    new Vector2(-2.970001f,-51.45647f),
    new Vector2(-2.970001f,-46.23647f),
    new Vector2(-2.970001f,-41.01647f),
    new Vector2(-7.534f,-41.01647f),
    new Vector2(-12.098f,-41.01647f),
    new Vector2(-16.662f,-41.01647f),
    new Vector2(-21.226f,-41.01647f),
    new Vector2(-25.78999f,-41.01647f),
    new Vector2(-25.78999f,-47.23647f),
    new Vector2(-30.65444f,-47.23647f),
    new Vector2(-35.51889f,-47.23647f),
    new Vector2(-40.38333f,-47.23647f),
    new Vector2(-45.24778f,-47.23647f),
    new Vector2(-50.11222f,-47.23647f),
    new Vector2(-54.97667f,-47.23647f),
    new Vector2(-59.84111f,-47.23647f),
    new Vector2(-64.70556f,-47.23647f),
    new Vector2(-69.57001f,-47.23647f),
    new Vector2(-69.57001f,-40.91147f),
    new Vector2(-69.57001f,-34.58647f),
    new Vector2(-75.39001f,-34.61647f),
    new Vector2(-81.21f,-34.64647f),
    new Vector2(-87.03f,-34.67647f),
    new Vector2(-87.03f,-29.52647f),
    new Vector2(-87.03f,-24.37647f),
    new Vector2(-87.03f,-19.22647f),
    new Vector2(-87.03f,-14.07647f),
    new Vector2(-87.03f,-8.926475f),
    new Vector2(-87.03f,-3.776474f),
    new Vector2(-94.14f,-3.776474f),
    new Vector2(-94.14f,1.014955f),
    new Vector2(-94.14f,5.806384f),
    new Vector2(-94.14f,10.59781f),
    new Vector2(-94.14f,15.38924f),
    new Vector2(-94.14f,20.18067f),
    new Vector2(-94.14f,24.9721f),
    new Vector2(-94.14f,29.76353f),
    new Vector2(-89.43546f,29.76353f),
    new Vector2(-84.73091f,29.76353f),
    new Vector2(-80.02636f,29.76353f),
    new Vector2(-75.32182f,29.76353f),
    new Vector2(-70.61727f,29.76353f),
    new Vector2(-65.91273f,29.76353f),
    new Vector2(-61.20818f,29.76353f),
    new Vector2(-56.50364f,29.76353f),
    new Vector2(-51.79909f,29.76353f),
    new Vector2(-47.09454f,29.76353f),
    new Vector2(-42.39f,29.76353f),
    new Vector2(-42.39f,24.37353f),
    new Vector2(-42.39f,18.98353f),
    new Vector2(-42.39f,13.59353f),
    new Vector2(-42.39f,8.203529f),
    new Vector2(-37.57273f,8.203529f),
    new Vector2(-32.75546f,8.203529f),
    new Vector2(-27.93818f,8.203529f),
    new Vector2(-23.12091f,8.203529f),
    new Vector2(-18.30364f,8.203529f),
    new Vector2(-13.48636f,8.203529f),
    new Vector2(-8.669092f,8.203529f),
    new Vector2(-3.851818f,8.203529f),
    new Vector2(0.9654531f,8.203529f),
    new Vector2(5.782727f,8.203529f),
    new Vector2(10.6f,8.203529f),
    new Vector2(10.6f,1.13353f),
    new Vector2(15.26923f,1.13353f),
    new Vector2(19.93846f,1.13353f),
    new Vector2(24.60769f,1.13353f),
    new Vector2(29.27692f,1.13353f),
    new Vector2(33.94616f,1.13353f),
    new Vector2(38.61539f,1.13353f),
    new Vector2(43.28462f,1.13353f),
    new Vector2(47.95385f,1.13353f),
    new Vector2(52.62308f,1.13353f),
    new Vector2(57.29231f,1.13353f),
    new Vector2(61.96154f,1.13353f),
    new Vector2(66.63078f,1.13353f),
    new Vector2(71.3f,1.13353f),
    new Vector2(71.3f,-3.51738f),
    new Vector2(71.3f,-8.168289f),
    new Vector2(71.3f,-12.8192f),
    new Vector2(71.3f,-17.47011f),
    new Vector2(71.3f,-22.12102f),
    new Vector2(71.3f,-26.77193f),
    new Vector2(71.3f,-31.42284f),
    new Vector2(71.3f,-36.07375f),
    new Vector2(71.3f,-40.72466f),
    new Vector2(71.3f,-45.37556f),
    new Vector2(71.3f,-50.02647f),
};

        Debug.Log($"Loaded hardcoded path with {_pathPositions.Length} positions");
    }

    void GenerateGrid()
    {
        // Generate path tiles
        foreach (var position in _pathPositions)
        {
            CreateTile(position);
        }

        // Set up path connections
        for (int i = 0; i < _pathPositions.Length; i++)
        {
            var currentTile = _tiles[_pathPositions[i]];

            // Add to ordered path list
            _pathTiles.Add(currentTile);

            // Set basic tile properties
            currentTile.Init(false);
            currentTile.SetColor(_pathColor);

            // Give each tile its path index
            currentTile.gameObject.name = $"Tile {i} ({_pathPositions[i].x}, {_pathPositions[i].y})";
        }

        // Set special tiles
        SetupSpecialTiles();
    }

    private void SetupSpecialTiles()
    {
        // Always make first tile a checkpoint (start position)
        if (_pathTiles.Count > 0)
        {
            _pathTiles[0].tileType = Tile.TileType.Checkpoint;
            _pathTiles[0].SetColor(_checkpointColor);
        }

        // Always make last tile a checkpoint (finish position)
        if (_pathTiles.Count > 1)
        {
            int lastIndex = _pathTiles.Count - 1;
            _pathTiles[lastIndex].tileType = Tile.TileType.Checkpoint;
            _pathTiles[lastIndex].SetColor(_checkpointColor);
        }

        // Randomly assign event and special tiles along the path
        // Skip the first and last tile (indices 0 and _pathTiles.Count - 1)
        for (int i = 2; i < _pathTiles.Count - 2; i++)
        {
            // Place checkpoints at regular intervals
            if (i % checkpointFrequency == 0)
            {
                _pathTiles[i].tileType = Tile.TileType.Checkpoint;
                _pathTiles[i].SetColor(_checkpointColor);
                continue;
            }

            // Place Minigame tiles at regular intervals
            if (i % minigameTilesFrequency == 0)
            {
                _pathTiles[i].tileType = Tile.TileType.Minigame;
                _pathTiles[i].SetColor(_minigameTileColor);
                continue;
            }

            // Randomly decide if this is an event tile or normal tile
            if (Random.value < eventTileChance)
            {
                _pathTiles[i].tileType = Tile.TileType.Event;
                _pathTiles[i].SetColor(_eventTileColor);
            }
        }

        // Ensure we have a minimum number of event tiles
        int eventTileCount = 0;
        foreach (var tile in _pathTiles)
        {
            if (tile.tileType == Tile.TileType.Event)
                eventTileCount++;
        }

        // Add more event tiles if we're below the minimum
        if (eventTileCount < minEventTiles)
        {
            int additionalTilesNeeded = minEventTiles - eventTileCount;
            List<int> normalTileIndices = new List<int>();

            // Find indices of normal tiles
            for (int i = 2; i < _pathTiles.Count - 2; i++)
            {
                if (_pathTiles[i].tileType == Tile.TileType.Normal)
                    normalTileIndices.Add(i);
            }

            // Shuffle the list for random selection
            ShuffleList(normalTileIndices);

            // Convert as many normal tiles to event tiles as needed
            for (int i = 0; i < additionalTilesNeeded && i < normalTileIndices.Count; i++)
            {
                int index = normalTileIndices[i];
                _pathTiles[index].tileType = Tile.TileType.Event;
                _pathTiles[index].SetColor(_eventTileColor);
            }
        }

        Debug.Log($"Set up special tiles: {eventTileCount} event tiles, {_pathTiles.Count} total tiles");
        // Always make last tile a checkpoint (finish position)
        if (_pathTiles.Count > 1)
        {
            int lastIndex = _pathTiles.Count - 1;
            _pathTiles[lastIndex].tileType = Tile.TileType.Checkpoint;
            _pathTiles[lastIndex].SetColor(_checkpointColor);
        }

    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void CreateTile(Vector2 position)
    {
        var spawnedTile = Instantiate(_tilePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        spawnedTile.transform.parent = transform;
        spawnedTile.name = $"Tile {position.x:F2} {position.y:F2}";
        _tiles.Add(position, spawnedTile);
    }

    // Get a tile at specific index along the path
    public Tile GetTileAtIndex(int index)
    {
        if (index >= 0 && index < _pathTiles.Count)
        {
            return _pathTiles[index];
        }
        return null;
    }

    // Get the position of a tile
    public Vector2 GetTilePosition(Tile tile)
    {
        foreach (var kvp in _tiles)
        {
            if (kvp.Value == tile)
            {
                return kvp.Key;
            }
        }
        return Vector2.zero;
    }

    // Get the next tile position from current position
    //public Vector2 GetNextTilePosition(Vector2 currentPos)
    //{
    //    // Find the closest path position
    //    int closestIndex = GetClosestPathIndex(currentPos);

    //    // Return the next position if available
    //    if (closestIndex >= 0 && closestIndex < _pathPositions.Length - 1)
    //    {
    //        return _pathPositions[closestIndex + 1];
    //    }

    //    return currentPos; // Return same position if at end
    //}

    // Find the index of the closest path position
    private int GetClosestPathIndex(Vector2 position)
    {
        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < _pathPositions.Length; i++)
        {
            float distance = Vector2.Distance(position, _pathPositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    //// Get tile at specific position
    //public Tile GetTileAtPosition(Vector2 position)
    //{
    //    // Try exact match first
    //    if (_tiles.ContainsKey(position))
    //    {
    //        return _tiles[position];
    //    }

    //    // If no exact match, find the closest tile within threshold
    //    float minDistance = float.MaxValue;
    //    Tile closestTile = null;

    //    foreach (var kvp in _tiles)
    //    {
    //        float distance = Vector2.Distance(position, kvp.Key);
    //        if (distance < minDistance && distance < _positionThreshold)
    //        {
    //            minDistance = distance;
    //            closestTile = kvp.Value;
    //        }
    //    }

    //    return closestTile;
    //}

    //// Get index of tile in path
    //public int GetTileIndex(Tile tile)
    //{
    //    return _pathTiles.IndexOf(tile);
    //}

    // Calculate path from one tile to another
    public List<Vector2> GetPathBetween(int startIndex, int endIndex)
    {
        List<Vector2> path = new List<Vector2>();

        // Validate indices
        if (startIndex < 0 || startIndex >= _pathTiles.Count ||
            endIndex < 0 || endIndex >= _pathTiles.Count)
        {
            Debug.LogError("Invalid tile indices");
            return path;
        }

        // Ensure we're moving forward on the path
        if (startIndex > endIndex)
        {
            Debug.LogWarning("Start index greater than end index, path will be empty");
            return path;
        }

        // Get positions along the path
        for (int i = startIndex; i <= endIndex; i++)
        {
            path.Add(GetTilePosition(_pathTiles[i]));
        }

        return path;
    }

    // Useful for debugging - highlight a specific tile
    public void HighlightTile(int index, Color highlightColor)
    {
        if (index >= 0 && index < _pathTiles.Count)
        {
            // Store original color
            Color originalColor = _pathTiles[index].GetComponent<SpriteRenderer>().color;

            // Highlight the tile
            _pathTiles[index].SetColor(highlightColor);

            // Reset after delay
            StartCoroutine(ResetTileColor(_pathTiles[index], originalColor, 1.0f));
        }
    }

    private IEnumerator ResetTileColor(Tile tile, Color originalColor, float delay)
    {
        yield return new WaitForSeconds(delay);
        tile.SetColor(originalColor);
    }
}