using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
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

    public Plug ProcessHit(ref RaycastHit hit, Gun source)
    {
        Plug plug = null;

        if (hit.collider.gameObject == gameObject)
        {
            Vector3 localNormal = hit.transform.InverseTransformVector(hit.normal);
            Vector3 localPos = hit.transform.InverseTransformPoint(hit.point);

            int index = GetHitSocketIndex(localNormal);
            Debug.Assert(index >= 0 && index < sockets.Length);

            plug = PlugManager.SpawnPlug(this, localPos, localNormal);

            plug.block = this;
            plug.type = sockets[index];

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
            return normal.x > 0.0f ? 3 : 2;
        }

        if (yAbs > xAbs && yAbs > zAbs)
        {
            return normal.y > 0.0f ? 5 : 4;
        }

        if (zAbs > xAbs && zAbs > yAbs)
        {
            return normal.z > 0.0f ? 1 : 0;
        }

        Utils.Unreachable();
        return -1;
    }
}
