using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private LayoutGroup grpPartsLayout;

    private Button[] partButtons;

    private void Awake()
    {
        Debug.Assert(grpPartsLayout != null);

        int partButtonCount = grpPartsLayout.transform.childCount;
        partButtons = new Button[partButtonCount];

        for (int i = 0; i < partButtonCount; i++)
        {
            var button = grpPartsLayout.transform.GetChild(i).gameObject.GetComponent<Button>();
            partButtons[i] = button;
        }
    }

    public void OnPartButtonPressed(GameObject partPrefab)
    {
        // TODO: Tool constant
        var placeTool = Desk.ToolManager.GetTool(1) as ToolPlacePart;
        Debug.Assert(placeTool != null);

        placeTool.SetPart(partPrefab);

        Desk.ToolManager.EnableTool(1);
    }
}
