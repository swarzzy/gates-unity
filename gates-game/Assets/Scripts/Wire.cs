using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Idea:
// To separate public parameters and internal system field we should
// put all public parameters in MonoBehaviour and keep all "private"
// parameters in data storage. For example data storage has Color
// field and in can be set through SetColor method in MonoBehaviour.
public class Wire : MonoBehaviour
{
    new public LineRenderer renderer;
    public Plug begin;
    public Plug end;
    public bool blocked;
    public int poolIndex = -1;
    public bool destroyed;
}
