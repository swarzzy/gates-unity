using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private static Collider2D[] tmpOvelapArray = new Collider2D[2];

    [SerializeField]
    private PartType type;

    [SerializeField]
    private SpriteRenderer bodyRenderer;

    [SerializeField]
    private TMP_Text label;

    [SerializeField]
    protected List<Pin> inputs = new List<Pin>();

    [SerializeField]
    protected List<Pin> outputs = new List<Pin>();

    public bool value;

    private Collider2D partCollider;

    private PartStyle style;

    public static void DestroyPart(Part part)
    {
        foreach (Pin input in part.inputs)
        {
            input.Unwire();
        }

        foreach (Pin output in part.outputs)
        {
            output.Unwire();
        }

        GameObject.Destroy(part.gameObject);
    }

    public void ChangeLocation(Vector2 newPos)
    {
        transform.position = (Vector3)newPos;

        foreach (Pin input in inputs)
        {
            foreach (Wire wire in input.GetWires())
            {
                wire.UpdatePositions();
            }
        }

        foreach (Pin output in outputs)
        {
            foreach (Wire wire in output.GetWires())
            {
                wire.UpdatePositions();
            }
        }
    }

    public void ApplyStyle(PartStyle newStyle)
    {
        if (style != newStyle)
        {
            var colors = Desk.Stylesheet.GetPartColors(newStyle);

            inputs.ForEach(it => it.GetRenderer().color = colors.inputColor);
            outputs.ForEach(it => it.GetRenderer().color = colors.outputColor);
            label.color = colors.labelColor;

            style = newStyle;
            UpdateBodyColor();
        }
    }

    private void UpdateBodyColor()
    {
        var colors = Desk.Stylesheet.GetPartColors(style);

        Color bodyColor = Color.gray;
        switch (type)
        {
            case PartType.Source: { bodyColor = value ? colors.bodyEnabledColor : colors.bodyDisabledColor; } break;
            case PartType.Led: { bodyColor = value ? colors.bodyEnabledColor : colors.bodyDisabledColor; } break;
            case PartType.Not: { bodyColor = colors.bodyEnabledColor; } break;
            case PartType.And:
            case PartType.Or:
            case PartType.Xor: { bodyColor = colors.bodyEnabledColor; } break;
            default: { Utils.Unreachable(); } break;
        }

        bodyRenderer.color = bodyColor;
    }

    public bool CheckOverlap()
    {
        bool result = false;
        Vector2 a = partCollider.bounds.min;
        Vector2 b = partCollider.bounds.max;

        int hitCount = Physics2D.OverlapAreaNonAlloc(a, b, tmpOvelapArray, (int)DeskLayer.PartMask);
        for (int i = 0; i < hitCount; i++)
        {
            if (tmpOvelapArray[i] == partCollider) continue;
            result = true;
        }

        return result;
    }

    public void OnClick()
    {
        switch (type)
        {
            case PartType.Source: {
                value = !value;
                UpdateBodyColor();
            } break;
            case PartType.Led: {} break;
            case PartType.Not: {} break;
            case PartType.And: {} break;
            case PartType.Or: {} break;
            case PartType.Xor: {} break;
            default: { Utils.Unreachable(); } break;
        }
    }

    private void Awake()
    {
        inputs.Clear();
        outputs.Clear();

        partCollider = GetComponent<BoxCollider2D>();
        Debug.Assert(partCollider != null);

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

        ApplyStyle(PartStyle.Normal);
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
                    UpdateBodyColor();
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
