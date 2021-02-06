using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    // true if another element was moved to free space
    public static bool EraseUnsorted<T>(this List<T> list, int index)
    {
        Debug.Assert(index < list.Count);

        if (list.Count == 1)
        {
            list.RemoveAt(index);
            return false;
        }

        int last = list.Count - 1;

        if (index == last)
        {
            list.RemoveAt(index);
            return false;
        }

        list[index] = list[last];
        list.RemoveAt(last);
        return true;
    }
}
