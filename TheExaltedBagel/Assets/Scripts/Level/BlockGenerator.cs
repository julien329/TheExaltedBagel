using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private GameObject blockObject;
    [SerializeField] private bool isDeathZone;
    [SerializeField] private bool isWater;
    [SerializeField] private bool useWaterTop;
    [SerializeField] private uint sizeX;
    [SerializeField] private uint sizeY;
    [SerializeField] private uint sizeZ;

    private GameObject blockObjectOld;
    private bool isDeathZoneOld;
    private bool isWaterOld;
    private bool useWaterTopOld;
    private uint sizeXOld;
    private uint sizeYOld;
    private uint sizeZOld;

    private const int MAX_VERTICES_COUNT = 65536;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnValidate()
    {
        if (!Application.isPlaying && this.gameObject.activeInHierarchy)
        {
            if (this.sizeX != this.sizeXOld || this.sizeY != this.sizeYOld || this.sizeZ != this.sizeZOld
                || this.blockObject != this.blockObjectOld || this.isDeathZone != this.isDeathZoneOld
                || this.isWater != this.isWaterOld || this.useWaterTop != this.useWaterTopOld)
            {
                this.sizeXOld = this.sizeX;
                this.sizeYOld = this.sizeX;
                this.sizeZOld = this.sizeZ;
                this.isDeathZoneOld = this.isDeathZone;
                this.isWaterOld = this.isWater;
                this.useWaterTopOld = this.useWaterTop;
                this.blockObjectOld = this.blockObject;

                StartCoroutine(RegenerateBlocks());
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator RegenerateBlocks()
    {
        yield return new WaitForEndOfFrame();

        DestroyPrevious();
        GenerateBlocks();

        if (this.sizeX > 0 && this.sizeY > 0 && this.sizeZ > 0)
        {
            ConfigureObject();
            CombineMeshes();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void GenerateBlocks()
    {
        if (this.blockObject != null)
        {
            if (this.isWater)
            {
                GameObject frontPlane = Instantiate(this.blockObject, this.transform);
                frontPlane.transform.gameObject.isStatic = true;
                frontPlane.transform.localPosition = new Vector3((this.sizeX / 2f) - 0.5f, (this.sizeY / 2f) - 0.5f, -0.25f);
                frontPlane.transform.localScale = new Vector3(this.sizeX + 1f, this.sizeY, 1f);
                frontPlane.name = "FrontPlane (" + this.sizeX + ", " + this.sizeY + ", " + this.sizeZ + ")";

                if (this.useWaterTop)
                {
                    GameObject topPlaneUp = Instantiate(this.blockObject, this.transform);
                    topPlaneUp.transform.gameObject.isStatic = true;
                    topPlaneUp.transform.localPosition = new Vector3((this.sizeX / 2f) - 0.5f, (this.sizeY - 0.5f), (this.sizeZ / 2f) - 0.5f);
                    topPlaneUp.transform.localScale = new Vector3(this.sizeX + 1f, this.sizeZ - 0.5f, 1f);
                    topPlaneUp.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    topPlaneUp.name = "TopPlaneUp (" + this.sizeX + ", " + 0 + ", " + this.sizeZ + ")";

                    GameObject topPlaneDown = Instantiate(this.blockObject, this.transform);
                    topPlaneDown.transform.gameObject.isStatic = true;
                    topPlaneDown.transform.localPosition = new Vector3((this.sizeX / 2f) - 0.5f, (this.sizeY - 0.5f), (this.sizeZ / 2f) - 0.5f);
                    topPlaneDown.transform.localScale = new Vector3(this.sizeX + 1f, this.sizeZ - 0.5f, 1f);
                    topPlaneDown.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
                    topPlaneDown.name = "TopPlaneDown (" + this.sizeX + ", " + 0 + ", " + this.sizeZ + ")";
                }
            }
            else
            {
                for (uint i = 0; i < this.sizeX; ++i)
                {
                    for (uint j = 0; j < this.sizeY; ++j)
                    {
                        for (uint k = 0; k < this.sizeZ; ++k)
                        {
                            if (i == 0 | j == 0 | k == 0 | i == this.sizeX - 1 | j == this.sizeY - 1)
                            {
                                GameObject newBlock = Instantiate(this.blockObject, this.transform);
                                newBlock.transform.gameObject.isStatic = true;
                                newBlock.transform.localPosition = new Vector3(i, j, k);
                                newBlock.name = "Block (" + i + ", " + j + ", " + k + ")";
                            }
                        }
                    }
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void DestroyPrevious()
    {
        while (this.transform.childCount > 0)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }

        BoxCollider boxCollider = this.gameObject.GetComponent<BoxCollider>();
        Rigidbody rigidbody = this.gameObject.GetComponent<Rigidbody>();

        DestroyImmediate(boxCollider);
        DestroyImmediate(rigidbody);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void ConfigureObject()
    {
        this.transform.gameObject.isStatic = true;

        BoxCollider boxCollider = this.gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        if (this.isWater)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Water");

            boxCollider.size = new Vector3(this.sizeX + 1f, this.sizeY, this.sizeZ);
            boxCollider.center = new Vector3((this.sizeX / 2f) - 0.5f, (this.sizeY / 2f) - 0.5f, (this.sizeZ / 2f) - 0.5f);
        }
        else if (this.isDeathZone)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Death");

            boxCollider.size = new Vector3(this.sizeX - 0.5f, this.sizeY - 0.5f, this.sizeZ);
            boxCollider.center = new Vector3((this.sizeX / 2f) - 0.5f, (this.sizeY / 2f), (this.sizeZ / 2f) - 0.5f);
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Floor");

            boxCollider.size = new Vector3(this.sizeX, this.sizeY, this.sizeZ);
            boxCollider.center = new Vector3((this.sizeX / 2f) - 0.5f, (this.sizeY / 2f), (this.sizeZ / 2f) - 0.5f);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void CombineMeshes()
    {
        Vector3 oldPos = this.transform.position;
        Quaternion oldRot = this.transform.rotation;

        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        int maxMeshCount = MAX_VERTICES_COUNT / meshFilters[0].sharedMesh.vertexCount;

        int combinedCount = 0;
        while (combinedCount < meshFilters.Length)
        {
            int currentMeshCount = Mathf.Min(maxMeshCount, meshFilters.Length - combinedCount);
            CombineInstance[] combineInstances = new CombineInstance[currentMeshCount];

            for (int i = 0; i < combineInstances.Length; ++i)
            {
                if (meshFilters[i].transform != this.transform)
                {
                    combineInstances[i].subMeshIndex = 0;
                    combineInstances[i].mesh = meshFilters[combinedCount + i].sharedMesh;
                    combineInstances[i].transform = meshFilters[combinedCount + i].transform.localToWorldMatrix;
                }
            }

            Mesh finalMesh = new Mesh();
            finalMesh.name = "MergedMesh";
            finalMesh.CombineMeshes(combineInstances);

            GameObject newObject = new GameObject("MergedMesh");
            newObject.transform.parent = this.gameObject.transform;
            newObject.transform.gameObject.isStatic = true;

            MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = finalMesh;

            MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
            meshRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

            combinedCount += currentMeshCount;
        }

        this.transform.position = oldPos;
        this.transform.rotation = oldRot;

        for (int i = meshFilters.Length - 1; i >= 0; --i)
        {
            DestroyImmediate(meshFilters[i].gameObject);
        }
    }
}
