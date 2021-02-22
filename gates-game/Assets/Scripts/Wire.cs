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

    new private BoxCollider2D collider;

    private Mesh mesh;

    public static void DestroyWire(Wire wire)
    {
        Debug.Assert(wire.begin != null);
        Debug.Assert(wire.end != null);

        wire.begin.RemoveWire(wire);
        wire.end.RemoveWire(wire);

        GameObject.Destroy(wire.gameObject);
    }

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

    public bool OverlapPoint(Vector3 point)
    {
        bool result = false;

        unsafe {

            Vector2 a = (Vector2)begin.transform.position;
            Vector2 b = (Vector2)end.transform.position;
            Vector2 diagonal = b - a;
            Vector2 perp = Vector2.Perpendicular(diagonal).normalized;

            Vector2* vertices = stackalloc Vector2[4];
            vertices[0] = a + perp * Desk.WireTestThickness;
            vertices[1] = b + perp * Desk.WireTestThickness;
            vertices[2] = b - perp * Desk.WireTestThickness;
            vertices[3] = a - perp * Desk.WireTestThickness;

            result = MathUtils.PointInPolygon2D(point, vertices, 4);
        }

        return result;
    }

    public void UpdatePositions()
    {
        lineRenderer.SetPosition(0, begin.transform.position);
        lineRenderer.SetPosition(1, end.transform.position);

        Vector3 diagonal = end.transform.position - begin.transform.position;
        Vector3 midPoint = begin.transform.position + diagonal * 0.5f;

        transform.position = midPoint;

        collider.size = diagonal;
        collider.offset = Vector2.zero;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Debug.Assert(lineRenderer != null);

        collider = GetComponent<BoxCollider2D>();
        Debug.Assert(collider != null);

        lineRenderer.positionCount = 2;

        StartCoroutine(UpdatePositionsLater());
    }

    private IEnumerator UpdatePositionsLater()
    {
        yield return new WaitForEndOfFrame();
        UpdatePositions();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying && (begin == null || end == null)) return;

        if (end.value != begin.value)
        {
            var colors = Desk.Stylesheet.GetWireColors();
            Color color = begin.value ? colors.enabledColor : colors.disabledColor;

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        end.value = begin.value;
    }
}
