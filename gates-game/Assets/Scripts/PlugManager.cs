using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugManager : PersistantSceneObject<PlugManager>
{
    [SerializeField]
    private float plugDespawnTimeout = 5.0f;

    public static float GetDespawnTimeout()
    {
        var manager = GetInstance();
        return manager.plugDespawnTimeout;
    }

    [SerializeField]
    private GameObject plugPrefab;

    private int plugSerialCount;

    public static Plug CreatePlug(Block block, ConnectionType connectionType, Vector3 p, Vector3 ori)
    {
        Plug plug = null;

        List<Plug> blockPlugs = null;
        int blockPlugsCap = Int32.MaxValue;

        switch (connectionType)
        {
            case ConnectionType.Input: { blockPlugs = block.inputs; blockPlugsCap = block.maxInputs; } break;
            case ConnectionType.Output: { blockPlugs = block.outputs; blockPlugsCap = block.maxOutputs;  } break;
            default: { blockPlugs = block.unconnected; } break;
        }

        if (blockPlugs.Count < blockPlugsCap)
        {
            var manager = GetInstance();
            var obj = Instantiate(manager.plugPrefab);
            obj.transform.SetParent(block.gameObject.transform, false);
            obj.transform.localPosition = p;
            obj.name = "Plug " + manager.plugSerialCount;

            plug = obj.GetComponent<Plug>();
            Debug.Assert(plug != null);
            plug.block = block;

            plug.type = connectionType;

            blockPlugs.Add(plug);
        }
        return plug;
    }
}
