using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Block : MonoBehaviour
{
    [SerializeField]
    private bool powered;

    protected bool powerWasChanged { get; private set; }

    public ConnectionType[] sockets = new ConnectionType[6];

    public int maxInputs = Int32.MaxValue;
    public int maxOutputs = Int32.MaxValue;

    public List<Plug> inputs { get; private set; } = new List<Plug>();
    public List<Plug> outputs { get; private set; } = new List<Plug>();
    public List<Plug> unconnected { get; private set; } = new List<Plug>();

    private MaterialPropertyBlock propertyBlock;
    private Renderer blockRenderer;

    public UnityEvent OnPowerRise;
    public UnityEvent OnPowerFall;

    protected virtual void OnTick() {}

    protected MaterialPropertyBlock GetPropertyBlock()
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
        return propertyBlock;
    }

    protected Renderer GetBlockRenderer()
    {
        if (blockRenderer == null) blockRenderer = GetComponent<MeshRenderer>();
        Debug.Assert(blockRenderer != null);
        return blockRenderer;
    }

    public void SetPower(bool power)
    {
        if (powered != power)
        {
            powered = power;
            UpdateMaterial();
            powerWasChanged = true;
        }
    }

    public bool GetPower() { return powered; }

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

    protected virtual void UpdateMaterial()
    {
        var property = GetPropertyBlock();
        var renderer = GetBlockRenderer();

        renderer.GetPropertyBlock(property);
        property.SetVector("_BlockSideIndices1", new Vector4((int)sockets[0], (int)sockets[1], (int)sockets[2], (int)sockets[3]));
        property.SetVector("_BlockSideIndices2", new Vector4((int)sockets[4], (int)sockets[5], 0, 0));
        renderer.SetPropertyBlock(property);
    }

    private void Update()
    {
        OnTick();

        if (powerWasChanged)
        {
            if (GetPower())
            {
                OnPowerRise?.Invoke();
            }
            else
            {
                OnPowerFall?.Invoke();
            }
        }

        powerWasChanged = false;
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateMaterialCoroutine());
    }

    private IEnumerator UpdateMaterialCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateMaterial();
    }

    private void OnValidate()
    {
        UpdateMaterial();
    }

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void OnDestroy()
    {
        foreach (var plug in GetAllPlugs())
        {
            plug.DestroyPlug(true);
            plug.block = null;
        }
    }
}
