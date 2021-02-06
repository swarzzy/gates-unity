using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockXor : LogicBlock
{
    private MaterialPropertyBlock propertyBlock;
    private Renderer blockRenderer;

    public override void Init()
    {
    }

    public override void Tick()
    {
        bool or = false;
        bool and = true;

        foreach (var plug in inputs)
        {
            or = or || plug.powered;
            and = and && plug.powered;
        }

        bool result = or && !and;

        foreach (var plug in outputs)
        {
            plug.powered = result;
        }

        Debug.Log("Tick");

        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
        if (blockRenderer == null) blockRenderer = GetComponent<MeshRenderer>();

        blockRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetVector("_BlockSideIndices1", new Vector4((int)sockets[0], (int)sockets[1], (int)sockets[2], (int)sockets[3]));
        propertyBlock.SetVector("_BlockSideIndices2", new Vector4((int)sockets[4], (int)sockets[5], 0, 0));
        blockRenderer.SetPropertyBlock(propertyBlock);
    }

}
