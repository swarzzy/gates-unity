using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plug : MonoBehaviour
{
    public enum State
    {
        Unwired = 0,
        Wired
    }

    public void DisableDespawn(bool disable)
    {
        disableDespawn = disable;
        timeUnwired = 0.0f;
    }

    public bool disableDespawn;
    public State state;
    public LogicBlock block;
    public ConnectionType type;
    public Wire wire;
    public int poolIndex = -1;
    public int id;
    public float timeUnwired;
    public bool destroyed;
}
