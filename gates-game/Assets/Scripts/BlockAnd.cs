using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAnd : Block
{
    protected override void OnTick()
    {
        bool and = (inputs.Count != 0);

        foreach (var plug in inputs)
        {
            and = and && plug.powered;
        }

        SetPower(and);

        foreach (var plug in outputs)
        {
            plug.powered = GetPower();
        }
    }
}
