using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float speedX = 0f;
    [SerializeField] float speedY = 0f;
    [SerializeField] float speedZ = 0f;

    void Update()
    {
        // Rotate the object around local X axis
        if (Mathf.Abs(this.speedX) > Mathf.Epsilon)
        {
            this.transform.Rotate(Vector3.right * Time.deltaTime * this.speedX);
        }

        // Rotate the object around local Y axis
        if (Mathf.Abs(this.speedY) > Mathf.Epsilon)
        {
            this.transform.Rotate(Vector3.up * Time.deltaTime * this.speedY);
        }

        // Rotate the object around local Z axis
        if (Mathf.Abs(this.speedZ) > Mathf.Epsilon)
        {
            this.transform.Rotate(Vector3.forward * Time.deltaTime * this.speedZ);
        }
    }
}
