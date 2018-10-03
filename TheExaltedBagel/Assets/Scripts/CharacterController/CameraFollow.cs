using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float followSpeed = 7f;

    private Transform player;
    private Vector3 targetPosition;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start () {
        player = GameObject.Find("Player").transform;
	}

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update () {
        targetPosition = player.position + cameraOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
	}
}
