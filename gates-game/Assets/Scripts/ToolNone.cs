using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolNone : Tool
{
    private Collider2D[] raycastBuffer = new Collider2D[32];

    private Wire wireUnderCursor;
    private Part partUnderCursor;
    private Pin pinUnderCursor;

    public Wire GetWireUnderCursor() { return wireUnderCursor; }
    public Part GetPartUnderCursor() { return partUnderCursor; }
    public Pin GetPinUnderCursor() { return pinUnderCursor; }

    public override void OnDisable(ToolContext context)
    {
        wireUnderCursor = null;
        partUnderCursor = null;
        pinUnderCursor = null;
    }

    public override void OnUpdate(ToolContext context)
    {
        bool processed = false;

        wireUnderCursor = null;
        partUnderCursor = null;
        pinUnderCursor = null;

        int hitCount = Physics2D.OverlapPointNonAlloc(context.mousePosWorld, raycastBuffer);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = raycastBuffer[i];

            if (hit.gameObject.layer == (int)DeskLayer.Pin)
            {
                Pin pin = hit.gameObject.GetComponent<Pin>();
                Debug.Assert(pin != null);

                pinUnderCursor = pin;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    var wireTool = context.manager.GetTool(2) as ToolWire;
                    Debug.Assert(wireTool != null);

                    bool canStartWire = wireTool.SetInitialPin(pin);
                    if (canStartWire)
                    {
                        context.manager.EnableTool(2);
                    }
                }

                processed = true;
                break;
            }
        }

        if (processed) return;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = raycastBuffer[i];
            if (hit.gameObject.layer == (int)DeskLayer.Part)
            {
                Part part = hit.gameObject.GetComponentInParent<Part>();
                Debug.Assert(part != null);

                partUnderCursor = part;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    part.OnClick();
                }
                else if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    if (!Desk.ControlManager.IsDragging())
                    {
                        Part.DestroyPart(part);
                    }
                }

                processed = true;
                break;
            }
        }

        if (processed) return;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = raycastBuffer[i];
            if (hit.gameObject.layer == (int)DeskLayer.Wire)
            {
                Wire wire = hit.gameObject.GetComponent<Wire>();
                Debug.Assert(wire != null);

                if (wire.OverlapPoint(context.mousePosWorld))
                {
                    wireUnderCursor = wire;

                    Debug.Log("Hit wire");
                    if (Input.GetKeyUp(KeyCode.Mouse1))
                    {
                        if (!Desk.ControlManager.IsDragging())
                        {
                            Wire.DestroyWire(wire);
                        }
                    }

                    processed = true;
                    break;
                }
            }
        }
    }
}
