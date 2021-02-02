using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField]
    private LineRenderer renderer;

    private Plug begin;

    private Plug end;

    public void SetPlugs(Plug b, Plug e)
    {
        begin = b;
        end = e;
    }

    private void Start()
    {
        renderer.positionCount = 2;
    }

    private void Update()
    {
        renderer.SetPosition(0, begin.transform.position);
        renderer.SetPosition(1, end.transform.position);
    }
}
