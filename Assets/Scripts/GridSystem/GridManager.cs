using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private Color _pathColor = Color.white;
    [SerializeField] private Color _specialTileColor = Color.yellow;

    // Define the path coordinates (can be modified in inspector)
    [SerializeField]
    private Vector2Int[] _pathPositions = new Vector2Int[]
    {
        new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(3,0),
        new Vector2Int(3,1), new Vector2Int(3,2), new Vector2Int(3,3),
        new Vector2Int(2,3), new Vector2Int(1,3), new Vector2Int(0,3),
        new Vector2Int(0,4), new Vector2Int(0,5), new Vector2Int(1,5),
        new Vector2Int(2,5), new Vector2Int(3,5), new Vector2Int(4,5),
        new Vector2Int(4,4), new Vector2Int(4,3), new Vector2Int(5,3),
        new Vector2Int(6,3), new Vector2Int(6,2), new Vector2Int(6,1),
        new Vector2Int(6,0), new Vector2Int(7,0) // End position
    };

    // Special tile positions (checkpoints, event spaces, etc.)
    [SerializeField]
    private Vector2Int[] _specialTilePositions = new Vector2Int[]
    {
        new Vector2Int(3,0), // Example checkpoint
        new Vector2Int(0,3), // Example event space
        new Vector2Int(6,3)  // Example special space
    };

    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();

    private void Start()
    {
        GenerateGrid();
        SetupCamera();
    }

    private void GenerateGrid()
    {
        // Generate only the path tiles
        foreach (var position in _pathPositions)
        {
            CreateTile(position);
        }

        // Set up tile connections (for path finding and movement)
        for (int i = 0; i < _pathPositions.Length - 1; i++)
        {
            var currentTile = _tiles[_pathPositions[i]];
            var nextTile = _tiles[_pathPositions[i + 1]];

            // You can store the next tile reference or setup visual indicators here
            // For demo purposes, we'll just set the color
            currentTile.Init(false);
            currentTile.SetColor(_pathColor);
        }

        // Mark special tiles
        foreach (var specialPos in _specialTilePositions)
        {
            if (_tiles.ContainsKey(specialPos))
            {
                _tiles[specialPos].SetColor(_specialTileColor);
            }
        }
    }

    private void CreateTile(Vector2Int position)
    {
        var spawnedTile = Instantiate(_tilePrefab, new Vector3(position.x, position.y), Quaternion.identity);
        spawnedTile.name = $"Tile {position.x} {position.y}";
        _tiles.Add(position, spawnedTile);
    }

    private void SetupCamera()
    {
        // Find the center of the path
        Vector2 sum = Vector2.zero;
        foreach (var pos in _pathPositions)
        {
            sum += new Vector2(pos.x, pos.y);
        }
        Vector2 center = sum / _pathPositions.Length;

        // Position camera to show the entire path
        _cam.transform.position = new Vector3(center.x, center.y, -10);
    }

    // Helper method to get next tile position (useful for player movement)
    public Vector2Int GetNextTilePosition(Vector2Int currentPos)
    {
        for (int i = 0; i < _pathPositions.Length - 1; i++)
        {
            if (_pathPositions[i] == currentPos)
                return _pathPositions[i + 1];
        }
        return currentPos; // Return same position if at end
    }
}