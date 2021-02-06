using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : PersistantSceneObject<WireManager>
{
    const int WireLayerMask = ~(1 << 7);

    public Color wireValidColor = Color.blue;
    public Color wireInvalidColor = Color.red;

    [SerializeField]
    private GameObject wirePrefab;

    [SerializeField]
    private List<Wire> wiresPool = new List<Wire>();

    [SerializeField]
    private List<Wire> wiresToDestroy = new List<Wire>();

    public static bool CanWirePlugs(Plug a, Plug b)
    {
        bool result = (a.type == ConnectionType.Input && b.type == ConnectionType.Output) || (a.type == ConnectionType.Output && b.type == ConnectionType.Input);
        return result;
    }

    public static Wire TryCreateWire(Plug begin, Plug end)
    {
        var manager = GetInstance();
        Wire wire = null;
        if (begin.block && end.block && (end.block != begin.block))
        {
            if (CanWirePlugs(begin, end))
            {
                var obj = Instantiate(manager.wirePrefab);
                wire = obj.GetComponent<Wire>();
                Debug.Assert(wire != null);

                wire.poolIndex = manager.wiresPool.Count;
                manager.wiresPool.Add(wire);

                wire.begin = begin.type == ConnectionType.Output ? begin : end;
                wire.end = begin.type == ConnectionType.Input ? begin : end;

                wire.renderer.positionCount = 2;

                bool beginWired = PlugManager.TryWirePlug(begin, wire);
                bool endWired = PlugManager.TryWirePlug(end, wire);

                Debug.Assert(beginWired);
                Debug.Assert(endWired);
            }
        }
        return wire;
    }

    public static void UnwireWire(Wire wire, Plug plug)
    {
        Debug.Assert(!(wire.begin == plug && wire.end == plug));

        if (wire.begin == plug)
        {
            wire.begin = null;
            DestroyWire(wire);
        }

        if (wire.end == plug)
        {
            wire.end = null;
            DestroyWire(wire);
        }
    }

    public static void DestroyWire(Wire wire)
    {
        Debug.Assert(!wire.destroyed);

        var manager = GetInstance();

        manager.wiresToDestroy.Add(wire);
        wire.destroyed = true;
    }

    private void ActuallyDestroyWire(Wire wire)
    {
        Debug.Assert(wire.destroyed);

        Plug begin = wire.begin;
        Plug end = wire.end;

        wire.begin = null;
        wire.end = null;

        if (begin != null) PlugManager.UnwirePlug(begin);
        if (end != null) PlugManager.UnwirePlug(end);


        Debug.Assert(wire.poolIndex < wiresPool.Count);
        Debug.Assert(wiresPool[wire.poolIndex] == wire);

        bool moved = wiresPool.EraseUnsorted(wire.poolIndex);
        if (moved)
        {
            var movedPlug = wiresPool[wire.poolIndex];
            movedPlug.poolIndex = wire.poolIndex;
        }

        Destroy(wire.gameObject);
    }

    private void Update()
    {
        foreach (var wire in wiresPool)
        {
            // TODO: Figure out a smarter way to do this!
            if (!wire.destroyed)
            {
                if(Physics.Linecast(wire.begin.transform.position, wire.end.transform.position, out RaycastHit hit, WireLayerMask))
                {
                    wire.blocked = true;
                    wire.renderer.SetPosition(0, wire.begin.transform.position);
                    wire.renderer.SetPosition(1, hit.point);
                    wire.renderer.startColor = wireInvalidColor;
                    wire.renderer.endColor = wireInvalidColor;
                }
                else
                {
                    wire.blocked = false;
                    wire.renderer.SetPosition(0, wire.begin.transform.position);
                    wire.renderer.SetPosition(1, wire.end.transform.position);
                    wire.renderer.startColor = wireValidColor;
                    wire.renderer.endColor = wireValidColor;
                }
            }
        }

        foreach(var wire in wiresToDestroy)
        {
            ActuallyDestroyWire(wire);
        }

        wiresToDestroy.Clear();
    }
}
