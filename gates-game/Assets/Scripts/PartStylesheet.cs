using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartStyleColors
{
    public Color bodyEnabledColor;
    public Color bodyDisabledColor;
    public Color labelColor;
    public Color inputColor;
    public Color outputColor;
}

public enum PartStyle
{
    Normal,
    Ghost,
    Invalid
}

[CreateAssetMenu(fileName = "PartStylesheet", menuName = "PartStylesheet", order = 1)]
public class PartStylesheet : ScriptableObject
{
    public PartStyleColors normalStyle;
    public PartStyleColors ghostStyle;
    public PartStyleColors invalidStyle;

    public PartStyleColors GetStyleColors(PartStyle style)
    {
        return style switch {
            PartStyle.Normal => normalStyle,
            PartStyle.Ghost => ghostStyle,
            PartStyle.Invalid => invalidStyle,
            _ => invalidStyle
        };
    }
}
