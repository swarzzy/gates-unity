using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockXor : Block
{
    protected override void OnTick()
    {
        bool or = false;
        bool and = true;

        foreach (var plug in inputs)
        {
            or = or || plug.powered;
            and = and && plug.powered;
        }

        SetPower(or && !and);

        foreach (var plug in outputs)
        {
            plug.powered = GetPower();
        }
    }
}
