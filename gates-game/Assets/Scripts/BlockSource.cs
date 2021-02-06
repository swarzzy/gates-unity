using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSource : LogicBlock
{
    public override void Init()
    {
    }

    public override void Tick()
    {
        UpdateMaterial(powered ? Color.yellow : Color.gray);

        foreach (var plug in outputs)
        {
            plug.powered = powered;
        }
    }

    private MaterialPropertyBlock propertyBlock;
    private Renderer blockRenderer;

    private void UpdateMaterial(Color color)
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
        if (blockRenderer == null) blockRenderer = GetComponent<MeshRenderer>();

        blockRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", color);
        blockRenderer.SetPropertyBlock(propertyBlock);
    }
}
