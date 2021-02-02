using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugManager : PersistantSceneObject<PlugManager>
{
    [SerializeField]
    private GameObject plugPrefab;

    public static Plug SpawnPlug(LogicBlock block, Vector3 p, Vector3 ori)
    {
        var obj = Instantiate(GetInstance().plugPrefab);
        obj.transform.SetParent(block.gameObject.transform, false);
        obj.transform.localPosition = p;

        var plug = obj.GetComponent<Plug>();
        Debug.Assert(plug != null);

        return plug;
    }
}
