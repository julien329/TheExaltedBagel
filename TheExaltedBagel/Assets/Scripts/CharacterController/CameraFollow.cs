using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float followSpeed = 7f;

    private Transform playerTranform;
    private Vector3 targetPosition;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake () {
        GameObject playerObject = GameObject.Find("Player");
        this.playerTranform = playerObject.transform;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update () {
        this.targetPosition = this.playerTranform.position + this.cameraOffset;

        this.transform.position = Vector3.Lerp(this.transform.position, this.targetPosition, this.followSpeed * Time.deltaTime);
	}
}
