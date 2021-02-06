using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    const int LogicLayer = (1 << 6) & (1 << 7);

    public Plug pendingPlug;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                var block = hit.collider.gameObject.GetComponent<LogicBlock>();
                if (block != null)
                {
                    Plug newPlug = block.ProcessHit(ref hit, this);
                    if (newPlug != null)
                    {
                        if (pendingPlug != null)
                        {
                            Wire wire = WireManager.TryCreateWire(newPlug, pendingPlug);
                            pendingPlug.DisableDespawn(false);
                            pendingPlug = null;
                        }
                        else
                        {
                            pendingPlug = newPlug;
                            pendingPlug.DisableDespawn(true);
                        }
                    }
                    return;
                }

                var plug = hit.collider.gameObject.GetComponent<Plug>();
                if (plug != null)
                {
                    PlugManager.DestroyPlug(plug);
                }
            }
        }
    }
}
