using UnityEngine;

public static class MovementUtils
{
    // Calculate point on a quadratic bezier curve
    public static Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float inverseT = 1 - t;  // Inverse of the time parameter
        float timeSquared = t * t;     // squared parameter for quadratic curve
        float inverseSquared = inverseT * inverseT;  // squared inverse parameter

        // Quadratic Bezier formula
        return inverseSquared * p0 + 2 * inverseT * t * p1 + timeSquared * p2;
    }

    // Get a control point for the bezier curve based on start and end positions
    public static Vector3 GetControlPoint(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
        float distance = Vector3.Distance(start, end);
        return (start + end) / 2f + perpendicular * (distance * 0.5f);
    }
}