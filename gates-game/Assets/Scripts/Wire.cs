using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wire : MonoBehaviour
{
    [SerializeField]
    private Pin begin; // Output

    [SerializeField]
    private Pin end; // input

    private LineRenderer lineRenderer;

    public static Wire CreateWire(Pin a, Pin b)
    {
        Wire result = null;

        if (a.GetPartRef() == b.GetPartRef()) return result;

        if ((a.GetPinType() == PinType.Input && b.GetPinType() == PinType.Output) ||
            (a.GetPinType() == PinType.Output && b.GetPinType() == PinType.Input))
        {
            Pin input = a.GetPinType() == PinType.Input ? a : b;
            Pin output = a.GetPinType() == PinType.Output ? a : b;
            Debug.Assert(input != output);

            int inputWireCount = input.GetWires().Count;
            if (inputWireCount == 0)
            {
                GameObject wireObj = GameObject.Instantiate(Desk.WirePrefab);
                Wire wire = wireObj.GetComponent<Wire>();
                Debug.Assert(wire != null);
                wire.begin = output;
                wire.end = input;

                bool outAdded = output.TryAddWire(wire);
                bool inpAdded = input.TryAddWire(wire);

                Debug.Assert(outAdded);
                Debug.Assert(inpAdded);

                result = wire;
            }
        }

        return result;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Debug.Assert(lineRenderer != null);

        lineRenderer.positionCount = 2;
    }

    private void LateUpdate()
    {
        lineRenderer.SetPosition(0, begin.transform.position);
        lineRenderer.SetPosition(1, end.transform.position);
        Color color = begin.value ? Color.red : Color.gray;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        end.value = begin.value;
    }
}
