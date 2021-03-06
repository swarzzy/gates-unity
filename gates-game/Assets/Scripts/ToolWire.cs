using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolWire : Tool
{
    private Pin initialPin;

    private Wire wire;

    private LineRenderer wireRenderer;

    public override void OnEnable(ToolContext context)
    {
        Desk.WireToolRenderer.SetActive(true);
        wireRenderer = Desk.WireToolRenderer.GetComponent<LineRenderer>();
        Debug.Assert(wireRenderer != null);
        wireRenderer.positionCount = 2;
    }

    public override void OnDisable(ToolContext context)
    {
        Desk.WireToolRenderer.SetActive(false);
    }

    public override void OnPrimaryButtonDown(ToolContext context)
    {
        Pin pin = Desk.GetPinUnderCursor();
        if (pin != null)
        {
            Wire wire = Wire.CreateWire(pin, initialPin);
            if (wire)
            {
                context.manager.EnableTool(0);
            }
        }
    }

    public override void OnUpdate(ToolContext context)
    {
        wireRenderer.SetPosition(0, initialPin.transform.position);
        wireRenderer.SetPosition(1, context.mousePosWorld);
    }

    public void SetInitialPin(Pin pin)
    {
        initialPin = pin;
    }

}