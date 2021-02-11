using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WireManager : PersistantSceneObject<WireManager>
{
    public Color wireValidColor = Color.blue;
    public Color wireInvalidColor = Color.red;

    public static Color WireValidColor { get { return GetInstance().wireValidColor; }}
    public static Color WireInvalidColor { get { return GetInstance().wireInvalidColor; }}

    [SerializeField]
    private GameObject wirePrefab;

    public static bool CanWirePlugs(Plug a, Plug b)
    {
        bool result = (a.type == ConnectionType.Input && b.type == ConnectionType.Output) || (a.type == ConnectionType.Output && b.type == ConnectionType.Input);
        return result;
    }

    public static bool TryWirePlug(Plug plug, Wire wire)
    {
        bool result = false;
        if (plug.wire == null)
        {
            if (plug.type == ConnectionType.Output && wire.begin == null)
            {
                plug.wire = wire;
                plug.timeUnwired = 0.0f;

                wire.begin = plug;
                wire.beginBlock = plug.block;

                result = true;
            }
            else if (plug.type == ConnectionType.Input && wire.end == null)
            {
                plug.wire = wire;
                plug.timeUnwired = 0.0f;

                wire.end = plug;
                wire.endBlock = plug.block;

                result = true;
            }
        }
        return result;
    }

    public static void UnwirePlug(Plug plug, Wire wire, bool doNotdestroyWire = false)
    {
        if (wire != null && plug != null)
        {
            if (wire.begin == plug)
            {
                wire.begin = null;
                wire.beginBlock = null;
                plug.wire = null;
            }
            else if (wire.end == plug)
            {
                wire.end = null;
                wire.endBlock = null;
                plug.wire = null;
            }

            if (!doNotdestroyWire)
            {
                if (wire.end == null || wire.begin == null)
                {
                    wire.DestroyWire();
                }
            }
        }
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

                wire.renderer.positionCount = 2;

                bool beginWired = TryWirePlug(begin, wire);
                bool endWired = TryWirePlug(end, wire);

                Debug.Assert(beginWired);
                Debug.Assert(endWired);
            }
        }
        return wire;
    }
}
