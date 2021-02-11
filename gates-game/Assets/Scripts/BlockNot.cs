using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockNot : Block
{
    protected override void OnTick()
    {
        Debug.Assert(inputs.Count <= 1);

        if (inputs.Count == 1)
        {
            SetPower(!inputs[0].powered);
        }

        foreach (var plug in outputs)
        {
            plug.powered = GetPower();
        }
    }
}
