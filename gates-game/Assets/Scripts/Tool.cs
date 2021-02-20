using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool
{
    public virtual void OnEnable(ToolContext context) {}
    public virtual void OnDisable(ToolContext context) { }

    public virtual void OnPrimaryButtonDown(ToolContext context) {}
    public virtual void OnPrimaryButtonUp(ToolContext context) {}

    public virtual void OnRightMouseDown(ToolContext context) {}
    public virtual void OnRightMouseUp(ToolContext context) {}

    public virtual void OnMiddleMouseDown(ToolContext context) {}
    public virtual void OnMiddleMouseUp(ToolContext context) {}

    public virtual bool OnDismiss(ToolContext context) { return true; }

    public virtual void OnUpdate(ToolContext context) {}
}
