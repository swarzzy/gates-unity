using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSource : Block
{
    private Color color;

    protected override void OnTick()
    {
        foreach (var plug in outputs)
        {
            plug.powered = GetPower();
        }

        color = GetPower() ? Color.yellow : Color.gray;

        UpdateMaterial();
    }

    protected override void UpdateMaterial()
    {
        var property = GetPropertyBlock();
        var renderer = GetBlockRenderer();

        renderer.GetPropertyBlock(property);
        property.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(property);
    }
}
