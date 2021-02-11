using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
    // Corresponds to index in texture array
    None = 0,
    Input,
    Output
}

[ExecuteInEditMode]
public class BlockManager : PersistantSceneObject<BlockManager>
{
    [SerializeField]
    private GameObject blockPrefab;

    private int blockSerialCount;

    public static Block CreateBlock()
    {
        var manager = GetInstance();

        var obj = Instantiate(manager.blockPrefab);
        obj.name = "Block " + manager.blockSerialCount++;

        var block = obj.GetComponent<Block>();
        Debug.Assert(block != null);

        return block;
    }
}
