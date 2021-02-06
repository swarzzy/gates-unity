using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugManager : PersistantSceneObject<PlugManager>
{
    public float plugDespawnTimeout = 5.0f;

    [SerializeField]
    private GameObject plugPrefab;

    [SerializeField]
    private List<Plug> unwiredPlugsPool = new List<Plug>();

    [SerializeField]
    private List<Plug> wiredPlugsPool = new List<Plug>();

    [SerializeField]
    private List<Plug> plugsToDestroy = new List<Plug>();

    private List<Plug> tmpPlugBuffer = new List<Plug>();

    private int plugSerialCount = 1;

    public static Plug CreatePlug(LogicBlock block, ConnectionType connectionType, Vector3 p, Vector3 ori)
    {
        var manager = GetInstance();
        var obj = Instantiate(manager.plugPrefab);
        obj.transform.SetParent(block.gameObject.transform, false);
        obj.transform.localPosition = p;
        obj.name = "Plug " + manager.plugSerialCount;

        var plug = obj.GetComponent<Plug>();
        Debug.Assert(plug != null);

        plug.id = manager.plugSerialCount++;
        plug.state = Plug.State.Unwired;
        plug.block = block;
        plug.type = connectionType;

        ChangePlugPool(plug, manager.unwiredPlugsPool);

        return plug;
    }

    public static bool TryWirePlug(Plug plug, Wire wire)
    {
        Debug.Assert(plug);

        bool result = false;
        var manager = GetInstance();

        // nocheckin ensure wire is not used already
        if (plug.poolIndex != -1 && plug.state != Plug.State.Wired && wire != null)
        {
            Debug.Assert(plug.wire == null);

            var pool = manager.GetPlugPool(plug);
            UnwirePlug(plug);
            ChangePlugPool(plug, manager.wiredPlugsPool);
            plug.state = Plug.State.Wired;
            plug.wire = wire;
            result = true;
        }

        return result;
    }

    public static void UnwirePlug(Plug plug)
    {
        Debug.Assert(plug);

        if (plug.state == Plug.State.Wired)
        {
            Debug.Assert(plug.wire != null);

            var manager = GetInstance();

            var pool = manager.GetPlugPool(plug);
            Debug.Assert(pool == manager.wiredPlugsPool);

            ChangePlugPool(plug, manager.unwiredPlugsPool);

            var wire = plug.wire;
            plug.state = Plug.State.Unwired;
            plug.wire = null;

            WireManager.UnwireWire(wire, plug);

        }
    }

    public static void DestroyPlug(Plug plug)
    {
        Debug.Assert(plug);

        //nochecking ValidatePlug()
        Debug.Assert(!plug.destroyed);

        var manager = GetInstance();
        manager.plugsToDestroy.Add(plug);
        plug.destroyed = true;
    }

    private List<Plug> GetPlugPool(Plug plug)
    {
        return plug.state switch
        {
            Plug.State.Unwired => unwiredPlugsPool,
            Plug.State.Wired => wiredPlugsPool,
            _ => Utils.Unreachable<List<Plug>>()
        };
    }

    private static void ChangePlugPool(Plug plug, List<Plug> newPool)
    {
        var manager = GetInstance();

        if (plug.poolIndex != -1)
        {
            var pool = manager.GetPlugPool(plug);
            Debug.Assert(plug.poolIndex < pool.Count);
            Debug.Assert(pool[plug.poolIndex] == plug);

            bool moved = pool.EraseUnsorted(plug.poolIndex);
            if (moved)
            {
                var movedPlug = pool[plug.poolIndex];
                movedPlug.poolIndex = plug.poolIndex;
            }
        }

        if (newPool != null)
        {
            int index = newPool.Count;
            newPool.Add(plug);
            plug.poolIndex = index;
        }
        else
        {
            plug.poolIndex = -1;
        }
    }

    private void ActuallyDestroyPlug(Plug plug)
    {
        Debug.Assert(plug.destroyed);
        Debug.Assert(plug);

        if (plug.wire)
        {
            WireManager.UnwireWire(plug.wire, plug);
        }

        if (plug.block != null)
        {
            plug.block.RemovePlug(plug);
        }

        var pool = GetPlugPool(plug);
        ChangePlugPool(plug, null);

        Destroy(plug.gameObject);
    }

    private void Update()
    {
        tmpPlugBuffer.Clear();

        foreach (var plug in unwiredPlugsPool)
        {
            // TODO: Scalable time
            plug.timeUnwired += Time.deltaTime;

            if (plug.disableDespawn) continue;

            if (plug.timeUnwired > plugDespawnTimeout)
            {
                tmpPlugBuffer.Add(plug);
            }
        }

        foreach(var plug in tmpPlugBuffer)
        {
            DestroyPlug(plug);
        }

        foreach(var plug in plugsToDestroy)
        {
            ActuallyDestroyPlug(plug);
        }

        plugsToDestroy.Clear();
    }
}
