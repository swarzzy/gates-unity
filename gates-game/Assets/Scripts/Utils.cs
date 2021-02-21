using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Utils
{
    public static T Unreachable<T>()
    {
        Debug.LogError("Invalid code path");
        return default(T);
    }

    public static void Unreachable()
    {
        Debug.LogError("Invalid code path");
    }

    public static T CreateDerivedByName<T>(string name) where T : class
    {
        T result = null;
        if (!string.IsNullOrEmpty(name))
        {
            Type type = Type.GetType(name);
            if (type != null)
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    result = (T)constructor.Invoke(new object[] {});
                }
            }
        }

        return result;
    }
}

public static class MathUtils
{
    // [https://rootllama.wordpress.com/2014/06/20/ray-line-segment-intersection-test-in-2d/]
    public static bool RayLineIntersect2D(Vector2 rayOrigin, Vector2 rayDir, Vector2 lineBegin, Vector2 lineEnd)
    {
        bool result = false;
        Vector2 perp = new Vector2(-rayDir.y, rayDir.x);
        Vector2 beginToOrigin = rayOrigin - lineBegin;
        Vector2 beginToEnd = lineEnd - lineBegin;

        float denom = Vector2.Dot(beginToEnd, perp);

        if (Mathf.Abs(denom) > Mathf.Epsilon)
        {
            float t1 = (beginToEnd.x * beginToOrigin.y - beginToOrigin.x * beginToEnd.y) / denom;
            float t2 = Vector2.Dot(beginToOrigin, perp) / denom;

            result = (t2 >= 0.0f && t2 <= 1.0f && t1 >= 0.0f);
        }

        return result;
    }

    // [https://rootllama.wordpress.com/2014/05/26/point-in-polygon-test/]
    unsafe public static bool PointInPolygon2D(Vector2 p, Vector2* vertices, int vertexCount)
    {
        bool result = false;
        int numCrossings = 0;

        for (int i = 0; i < vertexCount; i++)
        {
            int j = (i + 1) % vertexCount;

            if (RayLineIntersect2D(p, new Vector2(1.0f, 0.0f), vertices[i], vertices[j]))
            {
                numCrossings++;
            }
        }

        result = (numCrossings % 2) == 1;
        return result;
    }
}
