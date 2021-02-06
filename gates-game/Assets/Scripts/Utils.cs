using System.Collections;
using System.Collections.Generic;
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
}
