using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
    // Corresponds to index in texture array
    None = 0,
    Input,
    Output
}

public class LogicBlock : MonoBehaviour
{
    public ConnectionType[] sockets = new ConnectionType[6];

    public List<Plug> inputs = new List<Plug>();
    public List<Plug> outputs = new List<Plug>();
    public List<Plug> unconnected = new List<Plug>();

    private MaterialPropertyBlock uniformBlock;
    new private Renderer renderer;

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

            switch (plug.type)
            {
                case ConnectionType.Input: { inputs.Add(plug); } break;
                case ConnectionType.Output: { outputs.Add(plug); } break;
                default: { unconnected.Add(plug); } break;
            }

            Debug.Log($"Hit with index {index} of type {sockets[index].ToString()}");
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
        if (uniformBlock == null) uniformBlock = new MaterialPropertyBlock();
        if (renderer == null) renderer = GetComponent<MeshRenderer>();

        renderer.GetPropertyBlock(uniformBlock);
        uniformBlock.SetVector("_BlockSideIndices1", new Vector4((int)sockets[0], (int)sockets[1], (int)sockets[2], (int)sockets[3]));
        uniformBlock.SetVector("_BlockSideIndices2", new Vector4((int)sockets[4], (int)sockets[5], 0, 0));
        renderer.SetPropertyBlock(uniformBlock);
    }
}
