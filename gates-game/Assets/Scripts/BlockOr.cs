using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOr : Block
{
    protected override void OnTick()
    {
        bool or = false;

        foreach (var plug in inputs)
        {
            or = or || plug.powered;
        }

        SetPower(or);

        foreach (var plug in outputs)
        {
            plug.powered = GetPower();
        }
    }
}
