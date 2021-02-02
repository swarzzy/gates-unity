using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    const int LogicLayer = (1 << 6);

    public Plug pendingPlug;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, LogicLayer))
            {
                var block = hit.collider.gameObject.GetComponent<LogicBlock>();
                if (block != null)
                {
                    Plug plug = block.ProcessHit(ref hit, this);
                    if (plug != null)
                    {
                        if (pendingPlug != null)
                        {
                            WireManager.CreateWire(plug, pendingPlug);
                            pendingPlug = null;
                        }
                        else
                        {
                            pendingPlug = plug;
                        }
                    }
                }
            }
        }
    }
}
