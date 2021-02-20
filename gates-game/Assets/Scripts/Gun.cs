using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum Mode
    {
        Wire = 0,
        Block
    }

    const int LogicLayer = (1 << 6) & (1 << 7);

    [SerializeField]
    private Mode mode;

    public Plug pendingPlug;

    public GameObject blockMarkerPrefab;
    public GameObject blockPrefab;

    public Vector3 lastPlaceBlockPos;

    public GameObject blockMarkerInstance;

    public void SetMode(Mode newMode)
    {
        switch (mode)
        {
            case Mode.Wire: { WireModeEnd(); } break;
            case Mode.Block: { BlockModeEnd(); } break;
            default: { Utils.Unreachable(); } break;
        }

        mode = newMode;

        switch (mode)
        {
            case Mode.Wire: { WireModeBegin(); } break;
            case Mode.Block: { BlockModeBegin(); } break;
            default: { Utils.Unreachable(); } break;
        }
    }

    private void BlockModeBegin()
    {
        Debug.Assert(blockMarkerPrefab != null);
        Debug.Assert(blockPrefab != null);

        blockMarkerInstance = Instantiate(blockMarkerPrefab);
        blockMarkerInstance.SetActive(false);
    }

    private void BlockModeUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5.0f))
        {
            if (hit.collider.gameObject.GetComponent<BlockMarker>() == null)
            {
                // TODO: Clamp normal to axis-aliged directions
                Vector3 offset = hit.normal * 0.5f;
                lastPlaceBlockPos = hit.point + offset;
                blockMarkerInstance.transform.position = lastPlaceBlockPos;
                if (!blockMarkerInstance.activeSelf) blockMarkerInstance.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    Instantiate(blockPrefab, lastPlaceBlockPos, Quaternion.identity);
                }
            }
        }
        else
        {
            if (blockMarkerInstance.activeSelf) blockMarkerInstance.SetActive(false);
        }

    }

    private void BlockModeEnd()
    {
        if (blockMarkerInstance != null)
        {
            Destroy(blockMarkerInstance);
            blockMarkerInstance = null;
        }
    }

    private void WireModeBegin()
    {

    }

    private void WireModeUpdate()
    {
                if (Input.GetKeyDown(KeyCode.Space))
        {

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            var block = hit.collider.gameObject.GetComponent<Block>();
            if (block != null)
            {
                Plug newPlug = block.ProcessHit(ref hit, this);
                if (newPlug != null)
                {
                    if (pendingPlug != null)
                    {
                        _Wire wire = WireManager.TryCreateWire(newPlug, pendingPlug);
                        pendingPlug.disableDespawn = false;
                        pendingPlug = null;
                    }
                    else
                    {
                        pendingPlug = newPlug;
                        pendingPlug.disableDespawn = true;
                    }
                }
                return;
            }

            var plug = hit.collider.gameObject.GetComponent<Plug>();
            if (plug != null)
            {
                plug.DestroyPlug();
            }
        }
        }
    }

    private void WireModeEnd()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetMode(mode == Mode.Wire ? Mode.Block : Mode.Wire);
        }

        switch (mode)
        {
            case Mode.Wire: { WireModeUpdate(); } break;
            case Mode.Block: { BlockModeUpdate(); } break;
            default: { Utils.Unreachable(); } break;
        }
    }
}
