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
    new Vector2(38.66f,-58.97647f),
    new Vector2(38.66f,-57.40314f),
    new Vector2(38.66f,-55.8298f),
    new Vector2(38.66f,-54.25647f),
    new Vector2(37.27546f,-54.25647f),
    new Vector2(35.89091f,-54.25647f),
    new Vector2(34.50637f,-54.25647f),
    new Vector2(33.12182f,-54.25647f),
    new Vector2(31.73727f,-54.25647f),
    new Vector2(30.35273f,-54.25647f),
    new Vector2(28.96818f,-54.25647f),
    new Vector2(27.58364f,-54.25647f),
    new Vector2(26.19909f,-54.25647f),
    new Vector2(24.81455f,-54.25647f),
    new Vector2(23.43f,-54.25647f),
    new Vector2(23.43f,-52.86647f),
    new Vector2(23.43f,-51.47647f),
    new Vector2(22.11f,-51.47547f),
    new Vector2(20.79f,-51.47447f),
    new Vector2(19.47f,-51.47347f),
    new Vector2(18.15f,-51.47247f),
    new Vector2(16.83f,-51.47147f),
    new Vector2(15.51f,-51.47047f),
    new Vector2(14.19f,-51.46947f),
    new Vector2(12.87f,-51.46847f),
    new Vector2(11.55f,-51.46747f),
    new Vector2(10.23f,-51.46647f),
    new Vector2(8.909999f,-51.46547f),
    new Vector2(7.589999f,-51.46447f),
    new Vector2(6.27f,-51.46347f),
    new Vector2(4.949999f,-51.46247f),
    new Vector2(3.629999f,-51.46147f),
    new Vector2(2.309999f,-51.46047f),
    new Vector2(0.9899984f,-51.45947f),
    new Vector2(-0.3300004f,-51.45847f),
    new Vector2(-1.650001f,-51.45747f),
    new Vector2(-2.970001f,-51.45647f),
    new Vector2(-2.970001f,-50.15147f),
    new Vector2(-2.970001f,-48.84647f),
    new Vector2(-2.970001f,-47.54147f),
    new Vector2(-2.970001f,-46.23647f),
    new Vector2(-2.970001f,-44.93147f),
    new Vector2(-2.970001f,-43.62647f),
    new Vector2(-2.970001f,-42.32147f),
    new Vector2(-2.970001f,-41.01647f),
    new Vector2(-4.312354f,-41.01647f),
    new Vector2(-5.654706f,-41.01647f),
    new Vector2(-6.997059f,-41.01647f),
    new Vector2(-8.339411f,-41.01647f),
    new Vector2(-9.681764f,-41.01647f),
    new Vector2(-11.02412f,-41.01647f),
    new Vector2(-12.36647f,-41.01647f),
    new Vector2(-13.70882f,-41.01647f),
    new Vector2(-15.05117f,-41.01647f),
    new Vector2(-16.39353f,-41.01647f),
    new Vector2(-17.73588f,-41.01647f),
    new Vector2(-19.07823f,-41.01647f),
    new Vector2(-20.42058f,-41.01647f),
    new Vector2(-21.76294f,-41.01647f),
    new Vector2(-23.10529f,-41.01647f),
    new Vector2(-24.44764f,-41.01647f),
    new Vector2(-25.78999f,-41.01647f),
    new Vector2(-25.78999f,-42.57147f),
    new Vector2(-25.78999f,-44.12647f),
    new Vector2(-25.78999f,-45.68147f),
    new Vector2(-25.78999f,-47.23647f),
    new Vector2(-27.11666f,-47.23647f),
    new Vector2(-28.44333f,-47.23647f),
    new Vector2(-29.76999f,-47.23647f),
    new Vector2(-31.09666f,-47.23647f),
    new Vector2(-32.42333f,-47.23647f),
    new Vector2(-33.75f,-47.23647f),
    new Vector2(-35.07666f,-47.23647f),
    new Vector2(-36.40333f,-47.23647f),
    new Vector2(-37.73f,-47.23647f),
    new Vector2(-39.05666f,-47.23647f),
    new Vector2(-40.38333f,-47.23647f),
    new Vector2(-41.71f,-47.23647f),
    new Vector2(-43.03667f,-47.23647f),
    new Vector2(-44.36333f,-47.23647f),
    new Vector2(-45.69f,-47.23647f),
    new Vector2(-47.01667f,-47.23647f),
    new Vector2(-48.34333f,-47.23647f),
    new Vector2(-49.67f,-47.23647f),
    new Vector2(-50.99667f,-47.23647f),
    new Vector2(-52.32334f,-47.23647f),
    new Vector2(-53.65f,-47.23647f),
    new Vector2(-54.97667f,-47.23647f),
    new Vector2(-56.30334f,-47.23647f),
    new Vector2(-57.63f,-47.23647f),
    new Vector2(-58.95667f,-47.23647f),
    new Vector2(-60.28334f,-47.23647f),
    new Vector2(-61.61f,-47.23647f),
    new Vector2(-62.93667f,-47.23647f),
    new Vector2(-64.26334f,-47.23647f),
    new Vector2(-65.59f,-47.23647f),
    new Vector2(-66.91667f,-47.23647f),
    new Vector2(-68.24334f,-47.23647f),
    new Vector2(-69.57001f,-47.23647f),
    new Vector2(-69.57001f,-45.83092f),
    new Vector2(-69.57001f,-44.42536f),
    new Vector2(-69.57001f,-43.01981f),
    new Vector2(-69.57001f,-41.61425f),
    new Vector2(-69.57001f,-40.20869f),
    new Vector2(-69.57001f,-38.80314f),
    new Vector2(-69.57001f,-37.39758f),
    new Vector2(-69.57001f,-35.99203f),
    new Vector2(-69.57001f,-34.58647f),
    new Vector2(-70.91309f,-34.5934f),
    new Vector2(-72.25616f,-34.60032f),
    new Vector2(-73.59924f,-34.60724f),
    new Vector2(-74.94231f,-34.61416f),
    new Vector2(-76.28539f,-34.62109f),
    new Vector2(-77.62846f,-34.62801f),
    new Vector2(-78.97154f,-34.63493f),
    new Vector2(-80.31462f,-34.64186f),
    new Vector2(-81.65769f,-34.64878f),
    new Vector2(-83.00077f,-34.6557f),
    new Vector2(-84.34385f,-34.66262f),
    new Vector2(-85.68692f,-34.66955f),
    new Vector2(-87.03f,-34.67647f),
    new Vector2(-87.03f,-33.33299f),
    new Vector2(-87.03f,-31.98952f),
    new Vector2(-87.03f,-30.64604f),
    new Vector2(-87.03f,-29.30256f),
    new Vector2(-87.03f,-27.95908f),
    new Vector2(-87.03f,-26.6156f),
    new Vector2(-87.03f,-25.27213f),
    new Vector2(-87.03f,-23.92865f),
    new Vector2(-87.03f,-22.58517f),
    new Vector2(-87.03f,-21.24169f),
    new Vector2(-87.03f,-19.89821f),
    new Vector2(-87.03f,-18.55473f),
    new Vector2(-87.03f,-17.21126f),
    new Vector2(-87.03f,-15.86778f),
    new Vector2(-87.03f,-14.5243f),
    new Vector2(-87.03f,-13.18082f),
    new Vector2(-87.03f,-11.83734f),
    new Vector2(-87.03f,-10.49387f),
    new Vector2(-87.03f,-9.150387f),
    new Vector2(-87.03f,-7.80691f),
    new Vector2(-87.03f,-6.463429f),
    new Vector2(-87.03f,-5.119952f),
    new Vector2(-87.03f,-3.776474f),
    new Vector2(-88.452f,-3.776474f),
    new Vector2(-89.874f,-3.776474f),
    new Vector2(-91.296f,-3.776474f),
    new Vector2(-92.718f,-3.776474f),
    new Vector2(-94.14f,-3.776474f),
    new Vector2(-94.14f,-2.434874f),
    new Vector2(-94.14f,-1.093274f),
    new Vector2(-94.14f,0.248326f),
    new Vector2(-94.14f,1.589926f),
    new Vector2(-94.14f,2.931526f),
    new Vector2(-94.14f,4.273126f),
    new Vector2(-94.14f,5.614726f),
    new Vector2(-94.14f,6.956326f),
    new Vector2(-94.14f,8.297927f),
    new Vector2(-94.14f,9.639526f),
    new Vector2(-94.14f,10.98113f),
    new Vector2(-94.14f,12.32273f),
    new Vector2(-94.14f,13.66433f),
    new Vector2(-94.14f,15.00593f),
    new Vector2(-94.14f,16.34753f),
    new Vector2(-94.14f,17.68913f),
    new Vector2(-94.14f,19.03073f),
    new Vector2(-94.14f,20.37233f),
    new Vector2(-94.14f,21.71393f),
    new Vector2(-94.14f,23.05553f),
    new Vector2(-94.14f,24.39713f),
    new Vector2(-94.14f,25.73873f),
    new Vector2(-94.14f,27.08033f),
    new Vector2(-94.14f,28.42193f),
    new Vector2(-94.14f,29.76353f),
    new Vector2(-92.81308f,29.76353f),
    new Vector2(-91.48615f,29.76353f),
    new Vector2(-90.15923f,29.76353f),
    new Vector2(-88.83231f,29.76353f),
    new Vector2(-87.50539f,29.76353f),
    new Vector2(-86.17846f,29.76353f),
    new Vector2(-84.85154f,29.76353f),
    new Vector2(-83.52461f,29.76353f),
    new Vector2(-82.19769f,29.76353f),
    new Vector2(-80.87077f,29.76353f),
    new Vector2(-79.54385f,29.76353f),
    new Vector2(-78.21692f,29.76353f),
    new Vector2(-76.89f,29.76353f),
    new Vector2(-75.56307f,29.76353f),
    new Vector2(-74.23615f,29.76353f),
    new Vector2(-72.90923f,29.76353f),
    new Vector2(-71.58231f,29.76353f),
    new Vector2(-70.25539f,29.76353f),
    new Vector2(-68.92846f,29.76353f),
    new Vector2(-67.60154f,29.76353f),
    new Vector2(-66.27461f,29.76353f),
    new Vector2(-64.94769f,29.76353f),
    new Vector2(-63.62077f,29.76353f),
    new Vector2(-62.29385f,29.76353f),
    new Vector2(-60.96692f,29.76353f),
    new Vector2(-59.64f,29.76353f),
    new Vector2(-58.31308f,29.76353f),
    new Vector2(-56.98615f,29.76353f),
    new Vector2(-55.65923f,29.76353f),
    new Vector2(-54.33231f,29.76353f),
    new Vector2(-53.00538f,29.76353f),
    new Vector2(-51.67846f,29.76353f),
    new Vector2(-50.35154f,29.76353f),
    new Vector2(-49.02462f,29.76353f),
    new Vector2(-47.69769f,29.76353f),
    new Vector2(-46.37077f,29.76353f),
    new Vector2(-45.04385f,29.76353f),
    new Vector2(-43.71692f,29.76353f),
    new Vector2(-42.39f,29.76353f),
    new Vector2(-42.39f,28.41603f),
    new Vector2(-42.39f,27.06853f),
    new Vector2(-42.39f,25.72103f),
    new Vector2(-42.39f,24.37353f),
    new Vector2(-42.39f,23.02603f),
    new Vector2(-42.39f,21.67853f),
    new Vector2(-42.39f,20.33103f),
    new Vector2(-42.39f,18.98353f),
    new Vector2(-42.39f,17.63603f),
    new Vector2(-42.39f,16.28853f),
    new Vector2(-42.39f,14.94103f),
    new Vector2(-42.39f,13.59353f),
    new Vector2(-42.39f,12.24603f),
    new Vector2(-42.39f,10.89853f),
    new Vector2(-42.39f,9.551029f),
    new Vector2(-42.39f,8.203529f),
    new Vector2(-41.06525f,8.203529f),
    new Vector2(-39.7405f,8.203529f),
    new Vector2(-38.41575f,8.203529f),
    new Vector2(-37.091f,8.203529f),
    new Vector2(-35.76625f,8.203529f),
    new Vector2(-34.4415f,8.203529f),
    new Vector2(-33.11675f,8.203529f),
    new Vector2(-31.792f,8.203529f),
    new Vector2(-30.46725f,8.203529f),
    new Vector2(-29.1425f,8.203529f),
    new Vector2(-27.81775f,8.203529f),
    new Vector2(-26.493f,8.203529f),
    new Vector2(-25.16825f,8.203529f),
    new Vector2(-23.8435f,8.203529f),
    new Vector2(-22.51875f,8.203529f),
    new Vector2(-21.194f,8.203529f),
    new Vector2(-19.86925f,8.203529f),
    new Vector2(-18.5445f,8.203529f),
    new Vector2(-17.21975f,8.203529f),
    new Vector2(-15.895f,8.203529f),
    new Vector2(-14.57025f,8.203529f),
    new Vector2(-13.2455f,8.203529f),
    new Vector2(-11.92075f,8.203529f),
    new Vector2(-10.596f,8.203529f),
    new Vector2(-9.271251f,8.203529f),
    new Vector2(-7.946502f,8.203529f),
    new Vector2(-6.62175f,8.203529f),
    new Vector2(-5.297001f,8.203529f),
    new Vector2(-3.97225f,8.203529f),
    new Vector2(-2.647501f,8.203529f),
    new Vector2(-1.322752f,8.203529f),
    new Vector2(0.001999533f,8.203529f),
    new Vector2(1.326748f,8.203529f),
    new Vector2(2.6515f,8.203529f),
    new Vector2(3.976249f,8.203529f),
    new Vector2(5.300997f,8.203529f),
    new Vector2(6.625749f,8.203529f),
    new Vector2(7.950498f,8.203529f),
    new Vector2(9.275249f,8.203529f),
    new Vector2(10.6f,8.203529f),
    new Vector2(10.6f,6.789529f),
    new Vector2(10.6f,5.375529f),
    new Vector2(10.6f,3.961529f),
    new Vector2(10.6f,2.547529f),
    new Vector2(10.6f,1.13353f),
    new Vector2(11.91956f,1.13353f),
    new Vector2(13.23913f,1.13353f),
    new Vector2(14.55869f,1.13353f),
    new Vector2(15.87826f,1.13353f),
    new Vector2(17.19782f,1.13353f),
    new Vector2(18.51739f,1.13353f),
    new Vector2(19.83696f,1.13353f),
    new Vector2(21.15652f,1.13353f),
    new Vector2(22.47609f,1.13353f),
    new Vector2(23.79565f,1.13353f),
    new Vector2(25.11522f,1.13353f),
    new Vector2(26.43478f,1.13353f),
    new Vector2(27.75435f,1.13353f),
    new Vector2(29.07391f,1.13353f),
    new Vector2(30.39348f,1.13353f),
    new Vector2(31.71304f,1.13353f),
    new Vector2(33.03261f,1.13353f),
    new Vector2(34.35217f,1.13353f),
    new Vector2(35.67174f,1.13353f),
    new Vector2(36.9913f,1.13353f),
    new Vector2(38.31087f,1.13353f),
    new Vector2(39.63044f,1.13353f),
    new Vector2(40.95f,1.13353f),
    new Vector2(42.26957f,1.13353f),
    new Vector2(43.58913f,1.13353f),
    new Vector2(44.9087f,1.13353f),
    new Vector2(46.22826f,1.13353f),
    new Vector2(47.54782f,1.13353f),
    new Vector2(48.86739f,1.13353f),
    new Vector2(50.18696f,1.13353f),
    new Vector2(51.50652f,1.13353f),
    new Vector2(52.82609f,1.13353f),
    new Vector2(54.14565f,1.13353f),
    new Vector2(55.46522f,1.13353f),
    new Vector2(56.78479f,1.13353f),
    new Vector2(58.10435f,1.13353f),
    new Vector2(59.42392f,1.13353f),
    new Vector2(60.74348f,1.13353f),
    new Vector2(62.06305f,1.13353f),
    new Vector2(63.38261f,1.13353f),
    new Vector2(64.70218f,1.13353f),
    new Vector2(66.02174f,1.13353f),
    new Vector2(67.34131f,1.13353f),
    new Vector2(68.66087f,1.13353f),
    new Vector2(69.98044f,1.13353f),
    new Vector2(71.3f,1.13353f),
    new Vector2(71.3f,-0.1689094f),
    new Vector2(71.3f,-1.471348f),
    new Vector2(71.3f,-2.773787f),
    new Vector2(71.3f,-4.076226f),
    new Vector2(71.3f,-5.378666f),
    new Vector2(71.3f,-6.681104f),
    new Vector2(71.3f,-7.983544f),
    new Vector2(71.3f,-9.285982f),
    new Vector2(71.3f,-10.58842f),
    new Vector2(71.3f,-11.89086f),
    new Vector2(71.3f,-13.1933f),
    new Vector2(71.3f,-14.49574f),
    new Vector2(71.3f,-15.79818f),
    new Vector2(71.3f,-17.10062f),
    new Vector2(71.3f,-18.40306f),
    new Vector2(71.3f,-19.70549f),
    new Vector2(71.3f,-21.00793f),
    new Vector2(71.3f,-22.31037f),
    new Vector2(71.3f,-23.61281f),
    new Vector2(71.3f,-24.91525f),
    new Vector2(71.3f,-26.21769f),
    new Vector2(71.3f,-27.52013f),
    new Vector2(71.3f,-28.82257f),
    new Vector2(71.3f,-30.12501f),
    new Vector2(71.3f,-31.42745f),
    new Vector2(71.3f,-32.72989f),
    new Vector2(71.3f,-34.03233f),
    new Vector2(71.3f,-35.33476f),
    new Vector2(71.3f,-36.6372f),
    new Vector2(71.3f,-37.93964f),
    new Vector2(71.3f,-39.24208f),
    new Vector2(71.3f,-40.54452f),
    new Vector2(71.3f,-41.84696f),
    new Vector2(71.3f,-43.1494f),
    new Vector2(71.3f,-44.45184f),
    new Vector2(71.3f,-45.75428f),
    new Vector2(71.3f,-47.05671f),
    new Vector2(71.3f,-48.35915f),
    new Vector2(71.3f,-49.66159f),
    new Vector2(71.3f,-50.96404f),
    new Vector2(71.3f,-52.26647f),
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