using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolNone : Tool
{
    private Collider2D[] raycastBuffer = new Collider2D[32];

    public override void OnUpdate(ToolContext context)
    {
        bool processed = false;

        int hitCount = Physics2D.OverlapPointNonAlloc(context.mousePosWorld, raycastBuffer);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = raycastBuffer[i];

            if (hit.gameObject.layer == (int)DeskLayer.Pin)
            {
                Pin pin = hit.gameObject.GetComponent<Pin>();
                Debug.Assert(pin != null);

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

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    part.OnClick();
                }

                processed = true;
                break;
            }
        }
    }
}
