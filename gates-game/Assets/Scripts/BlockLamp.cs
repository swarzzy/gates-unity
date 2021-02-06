using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLamp : LogicBlock
{
    public override void Init()
    {
    }

    public override void Tick()
    {
        UpdateMaterial(powered ? Color.red : Color.gray);

        bool power = false;

        foreach (var plug in inputs)
        {
            power = power || plug.powered;
            if (power) break;
        }

        powered = power;
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
