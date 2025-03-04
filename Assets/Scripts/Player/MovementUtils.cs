using UnityEngine;

// Static utility class for movement calculations
public static class MovementUtils
{
    // Calculate point on a quadratic bezier curve
    public static Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float inverseT = 1 - t;  // Inverse of the time parameter
        float timeSquared = t * t;     // squared parameter for quadratic curve
        float inverseSquared = inverseT * inverseT;  // squared inverse parameter

        // Quadratic Bezier formula: (1-t)²*P₀ + 2(1-t)t*P₁ + t²*P₂
        return inverseSquared * p0 + 2 * inverseT * t * p1 + timeSquared * p2;
    }

    // Get a control point for the bezier curve based on start and end positions
    public static Vector3 GetControlPoint(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0); // Perpendicular vector
        float distance = Vector3.Distance(start, end);

        // Control point is midpoint between start and end, raised up by a factor of the distance
        return (start + end) / 2f + perpendicular * (distance * 0.5f);
    }

    // Calculate a curved path with multiple points
    public static Vector3[] CalculateCurvedPath(Vector3[] points, int resolution = 10)
    {
        if (points.Length < 2) return points;

        // For simple 2-point paths, just use a single bezier curve
        if (points.Length == 2)
        {
            Vector3[] result = new Vector3[resolution];
            Vector3 controlPoint = GetControlPoint(points[0], points[1]);

            for (int i = 0; i < resolution; i++)
            {
                float t = i / (float)(resolution - 1);
                result[i] = QuadraticBezier(points[0], controlPoint, points[1], t);
            }

            return result;
        }

        // For more complex paths, use multiple bezier segments
        Vector3[] path = new Vector3[(points.Length - 1) * resolution];

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            Vector3 control = GetControlPoint(start, end);

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)(resolution - 1);
                int index = i * resolution + j;

                // Don't add duplicate points where segments connect
                if (i > 0 && j == 0) continue;

                path[index] = QuadraticBezier(start, control, end, t);
            }
        }

        return path;
    }

    // Easing functions for smoother movement
    public static float EaseInOut(float t)
    {
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }

    public static float EaseOut(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }

    public static float BounceEase(float t, float frequency = 3f)
    {
        return Mathf.Sin(t * frequency * Mathf.PI) * (1 - t);
    }
}