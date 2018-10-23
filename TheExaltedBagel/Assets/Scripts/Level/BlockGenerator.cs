using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject blockObject;
    [SerializeField] private bool isWater;
    [SerializeField] private uint sizeX;
    [SerializeField] private uint sizeY;
    [SerializeField] private uint sizeZ;

    private GameObject blockObjectOld;
    private bool isWaterOld;
    private uint sizeXOld;
    private uint sizeYOld;
    private uint sizeZOld;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnValidate()
    {
        if (!Application.isPlaying && this.gameObject.activeInHierarchy)
        {
            if (this.sizeX != this.sizeXOld || this.sizeY != this.sizeYOld || this.sizeZ != this.sizeZOld 
                || this.blockObject != this.blockObjectOld || this.isWater != this.isWaterOld)
            {
                this.sizeXOld = this.sizeX;
                this.sizeYOld = this.sizeX;
                this.sizeZOld = this.sizeZ;
                this.isWaterOld = this.isWater;
                this.blockObjectOld = this.blockObject;

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

        BoxCollider boxCollider = this.gameObject.GetComponent<BoxCollider>();
        Rigidbody rigidbody = this.gameObject.GetComponent<Rigidbody>();

        DestroyImmediate(boxCollider);
        DestroyImmediate(rigidbody);

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

            if (this.transform.childCount > 0)
            {
                this.gameObject.layer = (this.isWater) ? LayerMask.NameToLayer("Water") : LayerMask.NameToLayer("Floor");

                boxCollider = this.gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = !this.isWater;
                boxCollider.size = new Vector3(this.sizeX, this.sizeY, this.sizeZ);
                boxCollider.center = new Vector3((this.sizeX - 1f) / 2f , ((this.sizeY - 1f) / 2f) + 0.5f, (this.sizeZ - 1f) / 2f);

                if (this.isWater)
                {
                    rigidbody = this.gameObject.AddComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
    }
}
