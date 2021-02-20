using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPlacePart : Tool
{
    private GameObject partPrefab;

    private GameObject part;

    public void SetPart(GameObject prefab)
    {
        partPrefab = prefab;
    }

    public override void OnEnable(ToolContext context)
    {
        Debug.Assert(part == null);
        part = GameObject.Instantiate(partPrefab, (Vector3)context.mousePosWorld, Quaternion.identity);
    }

    public override void OnPrimaryButtonDown(ToolContext context)
    {
        part = GameObject.Instantiate(partPrefab, (Vector3)context.mousePosWorld, Quaternion.identity);
    }

    public override void OnUpdate(ToolContext context)
    {
        part.transform.position = context.mousePosWorld;
    }

    public override void OnDisable(ToolContext context)
    {
        GameObject.Destroy(part);
        part = null;
    }

    public override bool OnDismiss(ToolContext context)
    {
        return true;
    }
}
