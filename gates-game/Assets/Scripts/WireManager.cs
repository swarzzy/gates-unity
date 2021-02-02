using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : PersistantSceneObject<WireManager>
{
    [SerializeField]
    private GameObject wirePrefab;

    public static Wire CreateWire(Plug begin, Plug end)
    {
        var obj = Instantiate(GetInstance().wirePrefab);
        var wire = obj.GetComponent<Wire>();
        Debug.Assert(wire != null);
        wire.SetPlugs(begin, end);
        return wire;
    }
}
