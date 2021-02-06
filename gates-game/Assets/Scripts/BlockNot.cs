using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockNot : LogicBlock
{
    private MaterialPropertyBlock propertyBlock;
    private Renderer blockRenderer;

    public override void Init()
    {
    }

    public override void Tick()
    {
        Debug.Assert(inputs.Count <= 1);

        bool value = false;

        if (inputs.Count == 1)
        {
            value = inputs[0];
        }

        foreach (var plug in outputs)
        {
            plug.powered = value;
        }

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
