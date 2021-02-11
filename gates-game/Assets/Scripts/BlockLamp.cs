using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLamp : Block
{
    private Color color;

    protected override void OnTick()
    {
        bool power = false;
        foreach (var plug in inputs)
        {
            power = power || plug.powered;
            if (power) break;
        }

        color = power ? Color.red : Color.gray;

        SetPower(power);
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
