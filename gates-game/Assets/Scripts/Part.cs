using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    Source = 0,
    Led,
    Not,
    And,
    Or,
    Xor
}

//[ExecuteInEditMode]
public class Part : MonoBehaviour
{
    [SerializeField]
    private PartType type;

    [SerializeField]
    private SpriteRenderer bodyRenderer;

    [SerializeField]
    protected List<Pin> inputs = new List<Pin>();

    [SerializeField]
    protected List<Pin> outputs = new List<Pin>();

    public bool value;

    public void OnClick()
    {
        switch (type)
        {
            case PartType.Source: {
                value = !value;
                bodyRenderer.color = value ? Color.red : Color.gray;
            } break;
            case PartType.Led: {} break;
            case PartType.Not: { } break;
            case PartType.And: { } break;
            case PartType.Or: { } break;
            case PartType.Xor: { } break;
            default: { Utils.Unreachable(); } break;
        }
    }

    private void Awake()
    {
        inputs.Clear();
        outputs.Clear();

        var pins = GetComponentsInChildren<Pin>();

        foreach(Pin pin in pins)
        {
            pin.SetPartRef(this);

            switch (pin.GetPinType())
            {
                case PinType.Input: { inputs.Add(pin); } break;
                case PinType.Output: { outputs.Add(pin); } break;
                default: { Utils.Unreachable(); } break;
            }
        }
    }

    private void Update()
    {
        switch (type)
        {
            case PartType.Source: {} break;

            case PartType.Led: {
                bool power = false;
                inputs.ForEach((it) => power = power || it.value);
                if (power != value)
                {
                    value = power;
                    bodyRenderer.color = value ? Color.red : Color.gray;
                }
            } break;

            case PartType.Not: {
                var power = false;
                inputs.ForEach((it) => power = power || it.value);
                value = !power;
            } break;

            case PartType.And: {
                var power = true;
                inputs.ForEach((it) => power = power && it.value);
                value = power;
            } break;

            case PartType.Or: {
                var power = false;
                inputs.ForEach((it) => power = power || it.value);
                value = power;
            } break;

            case PartType.Xor: {
                var or = false;
                var and = true;
                inputs.ForEach((it) => { or = or || it.value; and = and && and; });
                value = or && !and;
            } break;

            default: { Utils.Unreachable(); } break;
        }

        foreach (Pin output in outputs)
        {
            output.value = value;
        }
    }
}
