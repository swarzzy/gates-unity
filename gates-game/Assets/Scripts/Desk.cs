using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeskLayer
{
    Pin = 3,
    PinMask = (1 << 3),
    Part = 6,
    PartMask = (1 << 6)
}

public class Desk : PersistantSceneObject<Desk>
{
    [SerializeField]
    private ToolManager toolManager;

    [SerializeField]
    private GameObject wirePrefab;

    [SerializeField]
    private GameObject wireToolRenderer;

    public static ToolManager ToolManager { get {return GetInstance().toolManager; } }

    public static GameObject WirePrefab { get { return GetInstance().wirePrefab; } }

    public static GameObject WireToolRenderer { get { return GetInstance().wireToolRenderer; } }

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
