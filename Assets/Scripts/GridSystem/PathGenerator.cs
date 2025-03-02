using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathGenerator : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Color gizmoColor = Color.yellow;
    [SerializeField] private float gizmoSize = 0.3f;
    [SerializeField] private bool connectWithLines = true;
    [SerializeField] private Transform pathContainer; // Optional container for generated tiles

    // Path points stored in order
    public List<Transform> pathPoints = new List<Transform>();

    // Preview in Scene view
    private void OnDrawGizmos()
    {
        if (pathPoints.Count == 0 && transform.childCount > 0)
            CreatePathPoints();

        Gizmos.color = gizmoColor;

        // Draw points
        foreach (Transform point in pathPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, gizmoSize);
        }

        // Draw connecting lines
        if (connectWithLines)
        {
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                if (pathPoints[i] != null && pathPoints[i + 1] != null)
                    Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }
    }

    // Collect points from children
    [ContextMenu("Create Path Points")]
    void CreatePathPoints()
    {
        pathPoints.Clear();

        // Add all child objects as path points
        for (int i = 0; i < transform.childCount; i++)
        {
            pathPoints.Add(transform.GetChild(i));
        }

        Debug.Log($"Created {pathPoints.Count} path points from children");
    }

    // Generate actual tiles and path code
    [ContextMenu("Generate Path")]
    void GeneratePath()
    {
        if (pathPoints.Count == 0)
            CreatePathPoints();

        if (pathPoints.Count == 0)
        {
            Debug.LogError("No path points found!");
            return;
        }

        // Container for tiles if specified
        Transform container = pathContainer;
        if (container == null)
        {
            GameObject containerObj = new GameObject("Generated_Tiles");
            container = containerObj.transform;
        }

        // Dictionary to check for duplicate positions
        Dictionary<Vector2Int, bool> positionUsed = new Dictionary<Vector2Int, bool>();

        // Generate path code
        string pathCode = "private Vector2Int[] _pathPositions = new Vector2Int[]\n{\n";

        foreach (Transform point in pathPoints)
        {
            if (point == null) continue;

            Vector2Int pos = new Vector2Int(
                Mathf.RoundToInt(point.position.x),
                Mathf.RoundToInt(point.position.y)
            );

            // Check for duplicates
            if (positionUsed.ContainsKey(pos))
            {
                Debug.LogWarning($"Duplicate position found at {pos}, skipping");
                continue;
            }

            positionUsed[pos] = true;
            pathCode += $"    new Vector2Int({pos.x},{pos.y}),\n";

            // Create tile at position as normal tile
            Tile tile = Instantiate(tilePrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, container);
            tile.name = $"Tile {pos.x} {pos.y}";
            tile.gameObject.tag = "GameTile"; // Tag for easy finding later
            tile.tileType = Tile.TileType.Normal; // Explicitly set as normal
            tile.Init(false);
        }

        pathCode += "};";

        // Copy path code to clipboard
        GUIUtility.systemCopyBuffer = pathCode;

        Debug.Log($"Generated {positionUsed.Count} tiles");
        Debug.Log($"Path code copied to clipboard:\n{pathCode}");
    }

#if UNITY_EDITOR
    // Helper method to create a new path point
    [ContextMenu("Create Empty Point")]
    void CreateEmptyPoint()
    {
        GameObject point = new GameObject($"PathPoint_{transform.childCount}");
        point.transform.parent = transform;

        // If there are existing points, place the new one near the last one
        if (transform.childCount > 1)
        {
            Transform lastPoint = transform.GetChild(transform.childCount - 2);
            point.transform.position = lastPoint.position + Vector3.right;
        }
        else
        {
            point.transform.position = Vector3.zero;
        }

        // Add to path points list
        pathPoints.Add(point.transform);

        // Select the new point for easy movement
        Selection.activeGameObject = point;

        Debug.Log($"Created new path point: {point.name}");
    }

    // Helper to clear all generated tiles
    [ContextMenu("Clear Generated Tiles")]
    void ClearGeneratedTiles()
    {
        if (pathContainer != null)
        {
            while (pathContainer.childCount > 0)
            {
                DestroyImmediate(pathContainer.GetChild(0).gameObject);
            }
        }
        else
        {
            // Try to find the generated tiles container
            Transform container = GameObject.Find("Generated_Tiles")?.transform;
            if (container != null)
            {
                DestroyImmediate(container.gameObject);
            }
        }

        Debug.Log("Cleared all generated tiles");
    }
#endif
}