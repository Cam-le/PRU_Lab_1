using UnityEngine;
using System.Collections.Generic;

public class TileRandomizer : MonoBehaviour
{
    [System.Serializable]
    public class TileTypeDistribution
    {
        public Tile.TileType tileType;
        public Color tileColor = Color.white;

        [Range(0, 100)]
        public int probability = 10; // Probability in percentage

        [Tooltip("Ensure at least this many tiles of this type")]
        public int minimumCount = 0;

        [Tooltip("Don't exceed this many tiles of this type")]
        public int maximumCount = 999;
    }

    // Tile type distribution settings
    [SerializeField] private TileTypeDistribution[] tileTypes;

    // Optional settings
    [SerializeField] private bool randomizeOnAwake = true;
    [SerializeField] private bool excludeFirstTile = true;
    [SerializeField] private bool excludeLastTile = true;
    [SerializeField] private bool useConsistentSeed = false;
    [SerializeField] private int randomSeed = 0;

    // Start tile and end tile settings
    [SerializeField] private Tile.TileType startTileType = Tile.TileType.Checkpoint;
    [SerializeField] private Color startTileColor = Color.green;
    [SerializeField] private Tile.TileType endTileType = Tile.TileType.Checkpoint;
    [SerializeField] private Color endTileColor = Color.red;

    private void Awake()
    {
        if (randomizeOnAwake)
        {
            RandomizeTiles();
        }
    }

    [ContextMenu("Randomize Tiles Now")]
    public void RandomizeTiles()
    {
        // Initialize RNG with seed if specified
        if (useConsistentSeed)
        {
            Random.InitState(randomSeed);
        }

        // Find all game tiles
        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("GameTile");
        List<Tile> allTiles = new List<Tile>();

        foreach (GameObject obj in tileObjects)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile != null)
            {
                allTiles.Add(tile);
            }
        }

        Debug.Log($"Found {allTiles.Count} tiles to randomize");

        if (allTiles.Count == 0) return;

        // Sort tiles by name to ensure consistent ordering
        allTiles.Sort((a, b) => a.name.CompareTo(b.name));

        // Handle start and end tiles
        if (excludeFirstTile && allTiles.Count > 0)
        {
            Tile startTile = allTiles[0];
            startTile.tileType = startTileType;
            startTile.SetColor(startTileColor);
            allTiles.RemoveAt(0);
        }

        if (excludeLastTile && allTiles.Count > 0)
        {
            Tile endTile = allTiles[allTiles.Count - 1];
            endTile.tileType = endTileType;
            endTile.SetColor(endTileColor);
            allTiles.RemoveAt(allTiles.Count - 1);
        }

        // Create type tracking counts
        Dictionary<Tile.TileType, int> typeCounts = new Dictionary<Tile.TileType, int>();
        foreach (var tileTypeDist in tileTypes)
        {
            typeCounts[tileTypeDist.tileType] = 0;
        }

        // First pass: ensure minimum counts
        foreach (var tileTypeDist in tileTypes)
        {
            if (tileTypeDist.minimumCount > 0)
            {
                int count = Mathf.Min(tileTypeDist.minimumCount, allTiles.Count);

                for (int i = 0; i < count; i++)
                {
                    if (allTiles.Count == 0) break;

                    // Pick a random tile
                    int index = Random.Range(0, allTiles.Count);
                    Tile tile = allTiles[index];

                    // Set type and color
                    tile.tileType = tileTypeDist.tileType;
                    tile.SetColor(tileTypeDist.tileColor);

                    // Update tracking
                    typeCounts[tileTypeDist.tileType]++;
                    allTiles.RemoveAt(index);
                }
            }
        }

        // Second pass: randomize remaining tiles based on probability
        List<Tile> remainingTiles = new List<Tile>(allTiles);
        allTiles.Clear(); // Clear for reuse

        foreach (Tile tile in remainingTiles)
        {
            // Roll to determine tile type
            int roll = Random.Range(1, 101); // 1-100
            int cumulativeProbability = 0;
            bool assigned = false;

            foreach (var tileTypeDist in tileTypes)
            {
                cumulativeProbability += tileTypeDist.probability;

                if (roll <= cumulativeProbability &&
                    typeCounts[tileTypeDist.tileType] < tileTypeDist.maximumCount)
                {
                    tile.tileType = tileTypeDist.tileType;
                    tile.SetColor(tileTypeDist.tileColor);
                    typeCounts[tileTypeDist.tileType]++;
                    assigned = true;
                    break;
                }
            }

            // If no assignment made (due to probability or max counts), keep as normal
            if (!assigned)
            {
                tile.tileType = Tile.TileType.Normal;
                tile.SetColor(Color.white);
            }
        }

        // Log the distribution results
        string result = "Tile distribution results:\n";
        foreach (var entry in typeCounts)
        {
            result += $"- {entry.Key}: {entry.Value} tiles\n";
        }
        Debug.Log(result);
    }
}