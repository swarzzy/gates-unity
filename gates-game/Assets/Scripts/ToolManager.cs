using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolContext
{
    // Normalized
    public Vector2 mousePosScreen;
    public Vector2 mousePosWorld;
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
    private int defaultToolIndex;

    private Tool[] tools;

    private Tool currentTool;

    private ToolContext toolContext = new ToolContext();

    private Tool toolToSwitch;

    public ToolContext GetToolContext()
    {
        return toolContext;
    }

    public Tool GetTool(int index)
    {
        Debug.Assert(index >= 0 && index < tools.Length);
        Tool result = tools[index];
        return result;
    }

    public void EnableTool(int index)
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

        EnableTool(defaultToolIndex);
    }

    private void Update()
    {
        Vector3 mousePosRaw = Input.mousePosition;
        Vector3 mousePosClamped = new Vector3(Mathf.Clamp(mousePosRaw.x, 0.0f, Screen.width), Mathf.Clamp(mousePosRaw.y, 0.0f, Screen.height));
        toolContext.mousePosScreen = mousePosClamped / new Vector2(Screen.width, Screen.height);
        toolContext.mousePosWorld = toolContext.camera.ScreenToWorldPoint(new Vector3(mousePosClamped.x, mousePosClamped.y, toolContext.camera.nearClipPlane));

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (!Desk.ControlManager.IsDragging())
            {
                if (currentTool.OnDismiss(toolContext))
                {
                    EnableTool(defaultToolIndex);
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
