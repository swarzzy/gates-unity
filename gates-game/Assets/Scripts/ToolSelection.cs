using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelection : Tool
{
    private Part selection;

    private bool isDragging;

    private Vector2 dragOffset;

    public void SetSelection(Part part)
    {
        selection = part;
    }

    public override void OnEnable(ToolContext context)
    {
        Debug.Assert(selection != null);
    }

    public override void OnDisable(ToolContext context)
    {
        selection = null;
        dragOffset = Vector2.zero;
        isDragging = false;
    }

    public override void OnPrimaryButtonUp(ToolContext context)
    {
        if (!isDragging)
        {
            selection.OnClick();
        }

        context.manager.EnableTool(DeskTool.Default);
    }

    public override bool OnDismiss(ToolContext context)
    {
        return false;
    }

    public override void OnUpdate(ToolContext context)
    {
        float threshold = Desk.DragThresholdPx;
        float dragThresholdNormalizedSqrMag = Vector2.SqrMagnitude(new Vector2(threshold, threshold) / new Vector2(Screen.width, Screen.height));

        dragOffset += context.mouseScreenDelta;

        if (dragOffset.sqrMagnitude > dragThresholdNormalizedSqrMag)
        {
            isDragging = true;
        }

        if (isDragging)
        {
            Debug.Log("Dragging");
        }
    }
}
