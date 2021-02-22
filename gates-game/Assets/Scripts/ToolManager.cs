using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolContext
{
    // Normalized
    public Vector2 mousePosScreen;
    public Vector2 mousePosWorld;
    public Vector2 mouseScreenDelta;
    public Vector2 mouseWorldDelta;
    public Camera camera;
    public ToolManager manager;
}

public class ToolManager : MonoBehaviour
{
    [SerializeField]
    new private Camera camera;

    [SerializeField]
    private string[] toolsList;

    [SerializeField]
    private DeskTool defaultTool;

    private Tool[] tools;

    private Tool currentTool;

    private ToolContext toolContext = new ToolContext();

    private Tool toolToSwitch;

    private Vector2 lastMouseScreenPos;

    public ToolContext GetToolContext()
    {
        return toolContext;
    }

    public Tool GetDefaultTool()
    {
        Tool result = GetTool(defaultTool);
        return result;
    }

    public Tool GetTool(DeskTool index)
    {
        Debug.Assert((int)index >= 0 && (int)index < tools.Length);
        Tool result = tools[(int)index];
        return result;
    }

    public void EnableTool(DeskTool index)
    {
        Tool tool = GetTool(index);
        toolToSwitch = tool;
    }

    private void Awake()
    {
        Debug.Assert(camera != null);
        toolContext.camera = camera;
        toolContext.manager = this;

        tools = new Tool[toolsList.Length];

        for (int i = 0; i < toolsList.Length; i++)
        {
            var tool = Utils.CreateDerivedByName<Tool>(toolsList[i]);
            Debug.Assert(tool != null);
            tools[i] = tool;
        }

        Debug.Assert(tools != null);

        EnableTool(defaultTool);
    }

    private void Update()
    {
        Vector3 mousePosRaw = Input.mousePosition;
        Vector3 mousePosClamped = new Vector3(Mathf.Clamp(mousePosRaw.x, 0.0f, Screen.width), Mathf.Clamp(mousePosRaw.y, 0.0f, Screen.height));
        toolContext.mousePosScreen = mousePosClamped / new Vector2(Screen.width, Screen.height);
        toolContext.mousePosWorld = toolContext.camera.ScreenToWorldPoint(new Vector3(mousePosClamped.x, mousePosClamped.y, toolContext.camera.nearClipPlane));

        Vector2 mouseScreenDelta = toolContext.mousePosScreen - lastMouseScreenPos;

        Vector2 camViewportDim = new Vector2(camera.orthographicSize * camera.aspect * 2.0f, camera.orthographicSize * 2.0f);
        Vector2 mouseWorldDelta = camViewportDim * mouseScreenDelta;

        toolContext.mouseScreenDelta = mouseScreenDelta;
        toolContext.mouseWorldDelta = mouseWorldDelta;

        lastMouseScreenPos = toolContext.mousePosScreen;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (!Desk.ControlManager.IsDragging())
            {
                if (currentTool != tools[(int)defaultTool])
                {
                    if (currentTool.OnDismiss(toolContext))
                    {
                        EnableTool(defaultTool);
                    }
                }
            }
        }

        if (toolToSwitch == null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentTool.OnPrimaryButtonDown(toolContext);
        }

        if (toolToSwitch == null && Input.GetKeyUp(KeyCode.Mouse0))
        {
            currentTool.OnPrimaryButtonUp(toolContext);
        }

        if (toolToSwitch == null)
        {
            currentTool.OnUpdate(toolContext);
        }

        if (toolToSwitch != null)
        {
            if (currentTool != null) currentTool.OnDisable(toolContext);
            currentTool = toolToSwitch;
            currentTool.OnEnable(toolContext);

            toolToSwitch = null;
        }
    }
}
