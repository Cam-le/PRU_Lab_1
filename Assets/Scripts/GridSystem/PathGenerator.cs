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

    [Header("Tile Placement Settings")]
    [Tooltip("Distance between tiles along the path (can be decimal)")]
    [SerializeField] private float tileSpacing = 1.0f;
    [Tooltip("Whether to place tiles at exact integer grid positions")]
    [SerializeField] private bool snapToGrid = false; // Changed to false to support decimal positions
    [Tooltip("Whether to include the path control points as tiles")]
    [SerializeField] private bool includeControlPoints = true;

    // Path points stored in order
    public List<Transform> pathPoints = new List<Transform>();

    // Preview in Scene view
    private void OnDrawGizmos()
    {
        if (pathPoints.Count == 0 && transform.childCount > 0)
            CreatePathPoints();

        Gizmos.color = gizmoColor;

        // Draw control points
        foreach (Transform point in pathPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, gizmoSize);
        }

        // Draw connecting lines and intermediate points
        if (connectWithLines && pathPoints.Count > 1)
        {
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                if (pathPoints[i] == null || pathPoints[i + 1] == null) continue;

                Vector3 start = pathPoints[i].position;
                Vector3 end = pathPoints[i + 1].position;

                // Draw connecting line
                Gizmos.DrawLine(start, end);

                // Draw interpolated positions as smaller spheres
                float distance = Vector3.Distance(start, end);
                int subdivisions = Mathf.Max(1, Mathf.FloorToInt(distance / tileSpacing));

                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
                for (int j = 1; j < subdivisions; j++)
                {
                    float t = j / (float)subdivisions;
                    Vector3 interpolated = Vector3.Lerp(start, end, t);

                    // Draw interpolated position
                    Gizmos.DrawSphere(interpolated, gizmoSize * 0.5f);
                }

                // Restore original color
                Gizmos.color = gizmoColor;
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

        if (pathPoints.Count < 2)
        {
            Debug.LogError("Need at least 2 path points to generate a path!");
            return;
        }

        // Container for tiles if specified
        Transform container = pathContainer;
        if (container == null)
        {
            GameObject containerObj = new GameObject("Generated_Tiles");
            container = containerObj.transform;
        }

        // Use a different approach to track positions for decimal support
        Dictionary<Vector2, bool> positionUsed = new Dictionary<Vector2, bool>();
        List<Vector2> allPathPositions = new List<Vector2>();

        // Process each segment between control points
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            if (pathPoints[i] == null || pathPoints[i + 1] == null) continue;

            Vector3 start = pathPoints[i].position;
            Vector3 end = pathPoints[i + 1].position;

            // Calculate distance and subdivisions
            float distance = Vector3.Distance(start, end);
            int subdivisions = Mathf.Max(1, Mathf.FloorToInt(distance / tileSpacing));

            // Place tile at start point if it's the first segment or if includeControlPoints is true
            if (i == 0 || includeControlPoints)
            {
                Vector2 startPos;
                if (snapToGrid)
                {
                    startPos = new Vector2(
                        Mathf.Round(start.x),
                        Mathf.Round(start.y)
                    );
                }
                else
                {
                    startPos = new Vector2(start.x, start.y);
                }

                // Check for near duplicates when using decimal positions
                bool isDuplicate = false;
                foreach (Vector2 pos in positionUsed.Keys)
                {
                    if (Vector2.Distance(pos, startPos) < tileSpacing * 0.5f)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    CreateTileAtPosition(startPos, container);
                    positionUsed[startPos] = true;
                    allPathPositions.Add(startPos);
                }
            }

            // Place intermediate tiles
            for (int j = 1; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                Vector3 interpolated = Vector3.Lerp(start, end, t);

                Vector2 tilePos;
                if (snapToGrid)
                {
                    tilePos = new Vector2(
                        Mathf.Round(interpolated.x),
                        Mathf.Round(interpolated.y)
                    );
                }
                else
                {
                    tilePos = new Vector2(interpolated.x, interpolated.y);
                }

                // Check for near duplicates
                bool isDuplicate = false;
                foreach (Vector2 pos in positionUsed.Keys)
                {
                    if (Vector2.Distance(pos, tilePos) < tileSpacing * 0.5f)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    CreateTileAtPosition(tilePos, container);
                    positionUsed[tilePos] = true;
                    allPathPositions.Add(tilePos);
                }
            }

            // Handle the end point (for segments other than the last one)
            if (i < pathPoints.Count - 2) // Not the last segment
            {
                // Only add the end point if it's a control point
                if (includeControlPoints)
                {
                    Vector2 endPos;
                    if (snapToGrid)
                    {
                        endPos = new Vector2(
                            Mathf.Round(end.x),
                            Mathf.Round(end.y)
                        );
                    }
                    else
                    {
                        endPos = new Vector2(end.x, end.y);
                    }

                    // Check for near duplicates
                    bool isDuplicate = false;
                    foreach (Vector2 pos in positionUsed.Keys)
                    {
                        if (Vector2.Distance(pos, endPos) < tileSpacing * 0.5f)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (!isDuplicate)
                    {
                        CreateTileAtPosition(endPos, container);
                        positionUsed[endPos] = true;
                        allPathPositions.Add(endPos);
                    }
                }
            }
            else // Last segment - always add the final point
            {
                Vector2 endPos;
                if (snapToGrid)
                {
                    endPos = new Vector2(
                        Mathf.Round(end.x),
                        Mathf.Round(end.y)
                    );
                }
                else
                {
                    endPos = new Vector2(end.x, end.y);
                }

                // Check for near duplicates
                bool isDuplicate = false;
                foreach (Vector2 pos in positionUsed.Keys)
                {
                    if (Vector2.Distance(pos, endPos) < tileSpacing * 0.5f)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    CreateTileAtPosition(endPos, container);
                    positionUsed[endPos] = true;
                    allPathPositions.Add(endPos);
                }
            }
        }

        // Generate path code with support for decimal positions
        string pathCode = "";

        if (snapToGrid)
        {
            pathCode = "private Vector2Int[] _pathPositions = new Vector2Int[]\n{\n";
            foreach (Vector2 pos in allPathPositions)
            {
                pathCode += $"    new Vector2Int({(int)pos.x},{(int)pos.y}),\n";
            }
        }
        else
        {
            pathCode = "private Vector2[] _pathPositions = new Vector2[]\n{\n";
            foreach (Vector2 pos in allPathPositions)
            {
                pathCode += $"    new Vector2({pos.x}f,{pos.y}f),\n";
            }
        }

        pathCode += "};";

        // Copy path code to clipboard
        GUIUtility.systemCopyBuffer = pathCode;

        Debug.Log($"Generated {positionUsed.Count} tiles");
        Debug.Log($"Path code copied to clipboard");
    }

    private void CreateTileAtPosition(Vector2 position, Transform container)
    {
        Tile tile = Instantiate(tilePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, container);

        // Format name with decimal precision for non-grid positions
        if (snapToGrid)
        {
            tile.name = $"Tile {(int)position.x} {(int)position.y}";
        }
        else
        {
            tile.name = $"Tile {position.x:F2} {position.y:F2}";
        }

        tile.gameObject.tag = "GameTile"; // Tag for easy finding later
        tile.tileType = Tile.TileType.Normal; // Set as normal tile
        tile.Init(false);
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
            point.transform.position = lastPoint.position + Vector3.right * 2; // Place 2 units to the right
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

    // Preview tile placement in editor
    [ContextMenu("Preview Tile Placement")]
    void PreviewTilePlacement()
    {
        if (pathPoints.Count == 0)
            CreatePathPoints();

        int estimatedTileCount = 0;

        // Process each segment to estimate tiles
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            if (pathPoints[i] == null || pathPoints[i + 1] == null) continue;

            Vector3 start = pathPoints[i].position;
            Vector3 end = pathPoints[i + 1].position;

            float distance = Vector3.Distance(start, end);
            int subdivisions = Mathf.Max(1, Mathf.FloorToInt(distance / tileSpacing));

            // Count control points and subdivisions
            if (i == 0 || includeControlPoints)
                estimatedTileCount++; // Start point

            estimatedTileCount += subdivisions - 1; // Intermediate points

            if (i == pathPoints.Count - 2 || includeControlPoints)
                estimatedTileCount++; // End point
        }

        Debug.Log($"Preview: Path will generate approximately {estimatedTileCount} tiles");
        Debug.Log($"Tile spacing: {tileSpacing} units");
        Debug.Log($"Grid snapping: {(snapToGrid ? "Enabled" : "Disabled")}");
    }
#endif
}