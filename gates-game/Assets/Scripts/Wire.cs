using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Idea:
// To separate public parameters and internal system field we should
// put all public parameters in MonoBehaviour and keep all "private"
// parameters in data storage. For example data storage has Color
// field and in can be set through SetColor method in MonoBehaviour.
public class Wire : MonoBehaviour
{
    private const int WireLayerMask = ~(1 << 7);

    new public LineRenderer renderer;
    public Plug begin;
    public Plug end;
    public bool blocked;

    public void DestroyWire()
    {
        WireManager.UnwirePlug(begin, this, false);
        WireManager.UnwirePlug(end, this, false);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (Physics.Linecast(begin.transform.position, end.transform.position, out RaycastHit hit, WireLayerMask))
        {
            blocked = true;
            renderer.SetPosition(0, begin.transform.position);
            renderer.SetPosition(1, hit.point);
            renderer.startColor = WireManager.WireInvalidColor;
            renderer.endColor = WireManager.WireInvalidColor;
        }
        else
        {
            blocked = false;
            renderer.SetPosition(0, begin.transform.position);
            renderer.SetPosition(1, end.transform.position);
            renderer.startColor = WireManager.WireValidColor;
            renderer.endColor = WireManager.WireValidColor;
        }

        if (end != null && begin != null && !blocked)
        {
            end.powered = begin.powered;
        }
    }
}
