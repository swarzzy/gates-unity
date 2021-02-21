using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PinType
{
    Input, Output
}

public class Pin : MonoBehaviour
{
    private static List<Wire> unwireBuffer = new List<Wire>();

    [SerializeField]
    private PinType type;

    [SerializeField]
    private List<Wire> wires;

    private Part part;

    public bool value;

    public void Unwire()
    {
        unwireBuffer.Clear();
        unwireBuffer.AddRange(wires);

        foreach (Wire wire in unwireBuffer)
        {
            Wire.DestroyWire(wire);
        }
    }

    public void SetPartRef(Part p)
    {
        part = p;
    }

    public Part GetPartRef()
    {
        return part;
    }

    public IReadOnlyCollection<Wire> GetWires()
    {
        return wires;
    }

    public void RemoveWire(Wire wire)
    {
        wires.Remove(wire);
    }

    public bool TryAddWire(Wire wire)
    {
        bool result = false;
        if (type == PinType.Output || (type == PinType.Input && wires.Count == 0))
        {
            wires.Add(wire);
            result = true;
        }
        return result;
    }

    public PinType GetPinType()
    {
        return type;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Hit Pin");
    }
}
