using System;
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
