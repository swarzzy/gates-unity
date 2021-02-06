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

    [SerializeField]
    private List<LogicBlock> blocksPool = new List<LogicBlock>();

    private int blockSerialCount;

    public static LogicBlock CreateBlock()
    {
        var manager = GetInstance();

        var obj = Instantiate(manager.blockPrefab);
        obj.name = "Block " + manager.blockSerialCount++;

        var block = obj.GetComponent<LogicBlock>();
        Debug.Assert(block != null);

        return block;
    }

    public static void RegisterBlock(LogicBlock block)
    {
        int result = -1;
        var manager = GetInstanceOrNull();
        if (manager != null)
        {
            result = manager.blocksPool.Count;
            manager.blocksPool.Add(block);
        }
        block.poolIndex = result;
    }

    public static void UnregisterBlock(LogicBlock block)
    {
        var manager = GetInstanceOrNull();
        if (manager != null)
        {
            int index = block.poolIndex;
            if (index >= 0)
            {
                bool moved = manager.blocksPool.EraseUnsorted(index);
                if (moved)
                {
                    var movedBlock = manager.blocksPool[index];
                    movedBlock.poolIndex = index;
                }
            }
            block.poolIndex = -1;
        }
    }

    private void Update()
    {
        foreach (var block in blocksPool)
        {
            block.Tick();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        blocksPool.Clear();
    }

#if false
    private void ActuallyDestroyBlock(LogicBlock block)
    {
        Debug.Assert(block.destroyed);
        Debug.Assert(block.poolIndex != -1);

        var manager = GetInstance();

        Debug.Assert(block.poolIndex < blocksPool.Count);
        Debug.Assert(blocksPool[block.poolIndex] == block);

        foreach (var plug in block.GetAllPlugs())
        {
            PlugManager.DestroyPlug(plug);
            plug.block = null;
        }

        bool moved = blocksPool.EraseUnsorted(block.poolIndex);
        if (moved)
        {
            var movedPlug = blocksPool[block.poolIndex];
            movedPlug.poolIndex = block.poolIndex;
        }

        if (block)
        {
            // It may be already destroyed in editor mode
            Destroy(block.gameObject);
        }
    }


    private void LateUpdate()
    {
        foreach (var block in blocksToDestroy)
        {
            ActuallyDestroyBlock(block);
        }

        blocksToDestroy.Clear();
    }

#endif
}
