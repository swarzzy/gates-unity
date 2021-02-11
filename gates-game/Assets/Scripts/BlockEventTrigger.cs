using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockEventTrigger : Block
{
    public UnityEvent OnPowerRise;
    public UnityEvent OnPowerFall;

    protected override void OnTick()
    {
        if (powerWasChanged)
        {
            if (GetPower())
            {
                OnPowerRise?.Invoke();
            }
            else
            {
                OnPowerFall?.Invoke();
            }
        }
    }
}
