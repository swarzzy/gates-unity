using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPlacePart : Tool
{
    private GameObject partPrefab;

    private Part part;

    private bool overlapping;

    public void SetPart(GameObject prefab)
    {
        partPrefab = prefab;
    }

    public override void OnEnable(ToolContext context)
    {
        overlapping = false;

        Debug.Assert(part == null);
        var obj = GameObject.Instantiate(partPrefab, (Vector3)context.mousePosWorld, Quaternion.identity);
        part = obj.GetComponent<Part>();
        Debug.Assert(part != null);

        part.ApplyStyle(PartStyle.Ghost);

        OnUpdate(context);
    }

    public override void OnPrimaryButtonDown(ToolContext context)
    {
        if (!overlapping)
        {
            part.ApplyStyle(PartStyle.Normal);

            var obj = GameObject.Instantiate(partPrefab, (Vector3)context.mousePosWorld, Quaternion.identity);
            part = obj.GetComponent<Part>();
            Debug.Assert(part != null);

            overlapping = false;
            part.ApplyStyle(PartStyle.Ghost);
        }
    }

    public override void OnUpdate(ToolContext context)
    {
        part.transform.position = context.mousePosWorld;
        if (part.CheckOverlap())
        {
            if (!overlapping) part.ApplyStyle(PartStyle.Invalid);
            overlapping = true;
        }
        else
        {
            if (overlapping) part.ApplyStyle(PartStyle.Ghost);
            overlapping = false;
        }
    }

    public override void OnDisable(ToolContext context)
    {
        GameObject.Destroy(part.gameObject);
        part = null;
    }

    public override bool OnDismiss(ToolContext context)
    {
        return true;
    }
}
