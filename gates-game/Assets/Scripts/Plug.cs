using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plug : MonoBehaviour
{
    public bool disableDespawn;

    public Block block;
    public ConnectionType type;
    public Wire wire;
    public float timeUnwired;
    public bool powered;

    public void DestroyPlug(bool dontRemoveFromBlock = false)
    {
        if (wire != null)
        {
            WireManager.UnwirePlug(this, wire);
        }

        Debug.Assert(block != null);

        if (!dontRemoveFromBlock)
        {
            block.RemovePlug(this);
        }

        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (wire == null && !disableDespawn)
        {
            timeUnwired += Time.deltaTime;
        }

        if (timeUnwired > PlugManager.GetDespawnTimeout())
        {
            DestroyPlug();
        }
    }
}
