using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject blockObject;
    [SerializeField] private uint sizeX;
    [SerializeField] private uint sizeY;

    private GameObject blockObjectOld;
    private uint sizeXOld;
    private uint sizeYOld;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (sizeX != sizeXOld || sizeY != sizeYOld || blockObject != blockObjectOld)
            {
                sizeXOld = sizeX;
                sizeYOld = sizeX;
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
                    GameObject newBlock = Instantiate(this.blockObject, this.transform);
                    newBlock.transform.localPosition = new Vector3(i, j, 0f);
                    newBlock.name = "Block (" + i + ", " + j + ")";
                }
            }
        }
    }
}
