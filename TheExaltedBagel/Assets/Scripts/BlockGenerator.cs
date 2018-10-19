using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject blockObject;
    [SerializeField] private uint sizeX;
    [SerializeField] private uint sizeY;
    [SerializeField] private uint sizeZ;

    private GameObject blockObjectOld;
    private uint sizeXOld;
    private uint sizeYOld;
    private uint sizeZOld;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnValidate()
    {
        if (!Application.isPlaying && this.gameObject.activeInHierarchy)
        {
            if (sizeX != sizeXOld || sizeY != sizeYOld || sizeZ != sizeZOld || blockObject != blockObjectOld)
            {
                sizeXOld = sizeX;
                sizeYOld = sizeX;
                sizeZOld = sizeZ;
                blockObjectOld = blockObject;

                StartCoroutine(RegenerateBlocks());
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator RegenerateBlocks()
    {
        yield return new WaitForEndOfFrame();

        while (this.transform.childCount > 0)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }

        if (blockObject != null)
        {
            for (uint i = 0; i < this.sizeX; ++i)
            {
                for (uint j = 0; j < this.sizeY; ++j)
                {
                    for (uint k = 0; k < this.sizeZ; ++k)
                    {
                        GameObject newBlock = Instantiate(this.blockObject, this.transform);
                        newBlock.transform.localPosition = new Vector3(i, j, k);
                        newBlock.name = "Block (" + i + ", " + j + ", " + k + ")";
                    }
                }
            }
        }
    }
}
