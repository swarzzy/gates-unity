using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeskLayer
{
    Pin = 3,
    PartBody = 6,
    Wire = 8,
    Part = 9,
    PinMask = (1 << Pin),
    PartBodyMask = (1 << PartBody),
    WireMask = (1 << Wire),
    PartMask = (1 << Part)
}

public class Desk : PersistantSceneObject<Desk>
{
    [SerializeField]
    private PartStylesheet stylesheet;

    [SerializeField]
    private ToolManager toolManager;

    [SerializeField]
    private GameObject wirePrefab;

    [SerializeField]
    private GameObject wireToolRenderer;

    [SerializeField]
    private ControlManager controlManager;

    [SerializeField]
    private float wireTestThickness = 0.2f;

    public static ToolManager ToolManager { get { return GetInstance().toolManager; } }

    public static GameObject WirePrefab { get { return GetInstance().wirePrefab; } }

    public static GameObject WireToolRenderer { get { return GetInstance().wireToolRenderer; } }

    public static ControlManager ControlManager { get { return GetInstance().controlManager; } }

    public static float WireTestThickness { get { return GetInstance().wireTestThickness; } }

    public static PartStylesheet Stylesheet { get { return GetInstance().stylesheet; } }

    protected override void Awake()
    {
        base.Awake();
        // TODO: Cached properties
        Debug.Assert(toolManager != null);
    }

    public static Pin GetPinUnderCursor()
    {
        Pin result = null;
        Collider2D hit = Physics2D.OverlapPoint(ToolManager.GetToolContext().mousePosWorld);
        if (hit != null)
        {
            result = hit.gameObject.GetComponent<Pin>();
        }
        return result;
    }
}
