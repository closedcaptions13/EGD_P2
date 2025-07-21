using UnityEngine;

//* This was taken from Temperament *//

/// <summary>
/// General utilities class for mathematics.
/// </summary>
public static class MathUtil
{
    public static Vector2 DegreesToVector2(float degrees)
        => Vector2.up.Rotate(Mathf.Deg2Rad * degrees);

    public static Vector2 ReduceDimension(Vector3 value, Vector3 right, Vector3 up)
        => new(Vector3.Dot(value, right), Vector3.Dot(value, up));

    /// <summary>
    /// Inverse lerp without unity's clamping between zero and one.
    /// </summary>
    public static float InverseLerpUnclamped(float a, float b, float v)
        => (v - a) / (b - a);

    /// <summary>
    /// Rounds the first value to increments of the second.
    /// </summary>
    public static float RoundTo(float value, float step)
        => Mathf.Round(value / step) * step;


    /// <summary>
    /// Return the instersection between two lines defined by the provided pairs of points.
    /// <see href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
    /// </summary>
    public static Vector2 Intersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        var denom = (a1.x - a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x - b2.x);

        var unscaled = (a1.x * a2.y - a1.y * a2.x) * (b1 - b2) - (a1 - a2) * (b1.x * b2.y - b1.y * b2.x);
        return unscaled / denom;
    }

    // https://matthew-brett.github.io/teaching/rotation_2d.html //
    /// <summary>
    /// Rotate a vector by the provided angle in radians.
    /// <see href="https://matthew-brett.github.io/teaching/rotation_2d.html"/>
    /// </summary>
    public static Vector2 Rotate(this Vector2 xy, float angle)
    {
        var c = Mathf.Cos(angle);
        var s = Mathf.Sin(angle);

        return new(
            c * xy.x - s * xy.y,
            s * xy.x + c * xy.y
        );
    }

    /// <summary>
    /// Helper function to check if a value lies between those provided.
    /// </summary>
    public static bool Between(this float x, float min, float max)
        => min <= x && x <= max;

    /// <summary>
    /// Helper function to check if a value lies between those provided.
    /// </summary>
    public static bool Between(this int x, int min, int max)
        => min <= x && x <= max;

    /// <summary>
    /// Helper function to check if two intervals have any intersection.
    /// </summary>
    public static bool RangesIntersect(float amin, float amax, float bmin, float bmax)
    {
        return amin.Between(bmin, bmax) || amax.Between(bmin, bmax)
            || bmin.Between(amin, amax) || bmax.Between(amin, amax);
    }

    /// <summary>
    /// Helper function to check if two intervals have any intersection.
    /// </summary>
    public static bool RangesIntersect(int amin, int amax, int bmin, int bmax)
    {
        return amin.Between(bmin, bmax) || amax.Between(bmin, bmax)
            || bmin.Between(amin, amax) || bmax.Between(amin, amax);
    }

    /// <summary>
    /// Ease the provided value towards another, accounting for delta time properly.
    /// </summary>
    public static float EaseTowards(float value, float target, float sharpness, float dt)
    {
        if (Mathf.Abs(value - target) < Mathf.Epsilon)
            return target;

        var x0 = Mathf.Log(Mathf.Abs(value - target)) / -sharpness;
        var result = target + Mathf.Sign(value - target) * Mathf.Exp(-sharpness * (x0 + dt));

        return result;
    }
}
