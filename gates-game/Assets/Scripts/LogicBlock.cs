using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class LogicBlock : MonoBehaviour
{
    public bool powered;

    [System.NonSerialized]
    public int poolIndex = -1;

    [System.NonSerialized]
    public bool destroyed;

    public ConnectionType[] sockets = new ConnectionType[6];

    [System.NonSerialized]
    public List<Plug> inputs = new List<Plug>();

    public int maxInputs = Int32.MaxValue;
    public int maxOutputs = Int32.MaxValue;

    [System.NonSerialized]
    public List<Plug> outputs = new List<Plug>();

    [System.NonSerialized]
    public List<Plug> unconnected = new List<Plug>();

    private MaterialPropertyBlock uniformBlock;
    new private Renderer renderer;

    public abstract void Init();
    public abstract void Tick();

    private void Awake()
    {
        BlockManager.RegisterBlock(this);
        Init();
    }

    private void OnDestroy()
    {
        BlockManager.UnregisterBlock(this);

        foreach (var plug in GetAllPlugs())
        {
            plug.DestroyPlug(true);
            plug.block = null;
        }
    }

    public IEnumerable<Plug> GetAllPlugs()
    {
        foreach (var plug in inputs)
        {
            yield return plug;
        }

        foreach (var plug in outputs)
        {
            yield return plug;
        }

        foreach (var plug in unconnected)
        {
            yield return plug;
        }
    }

    public void RemovePlug(Plug plug)
    {
        if (inputs.Remove(plug)) return;
        if (outputs.Remove(plug)) return;
        if (unconnected.Remove(plug)) return;
        Utils.Unreachable();
    }

    public Plug ProcessHit(ref RaycastHit hit, Gun source)
    {
        Plug plug = null;

        if (hit.collider.gameObject == gameObject)
        {
            Vector3 localNormal = hit.transform.InverseTransformVector(hit.normal);
            Vector3 localPos = hit.transform.InverseTransformPoint(hit.point) + localNormal * 0.1f;

            int index = GetHitSocketIndex(localNormal);
            Debug.Assert(index >= 0 && index < sockets.Length);

            plug = PlugManager.CreatePlug(this, sockets[index], localPos, localNormal);
        }

        return plug;
    }

    private int GetHitSocketIndex(Vector3 normal)
    {
        float xAbs = Mathf.Abs(normal.x);
        float yAbs = Mathf.Abs(normal.y);
        float zAbs = Mathf.Abs(normal.z);

        if (xAbs > yAbs && xAbs > zAbs)
        {
            return normal.x > 0.0f ? 1 : 0;
        }

        if (yAbs > xAbs && yAbs > zAbs)
        {
            return normal.y > 0.0f ? 3 : 2;
        }

        if (zAbs > xAbs && zAbs > yAbs)
        {
            return normal.z > 0.0f ? 5 : 4;
        }

        Utils.Unreachable();
        return -1;
    }

    private void UpdateMaterial()
    {
        #if false
        if (uniformBlock == null) uniformBlock = new MaterialPropertyBlock();
        if (renderer == null) renderer = GetComponent<MeshRenderer>();

        renderer.GetPropertyBlock(uniformBlock);
        uniformBlock.SetVector("_BlockSideIndices1", new Vector4((int)sockets[0], (int)sockets[1], (int)sockets[2], (int)sockets[3]));
        uniformBlock.SetVector("_BlockSideIndices2", new Vector4((int)sockets[4], (int)sockets[5], 0, 0));
        renderer.SetPropertyBlock(uniformBlock);
        #endif
    }

    private void OnEnable()
    {
        uniformBlock = new MaterialPropertyBlock();
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UpdateMaterial();
    }

    private void OnValidate()
    {
        UpdateMaterial();
    }
}
