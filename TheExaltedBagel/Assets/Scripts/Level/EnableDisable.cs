using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisable : MonoBehaviour {

    enum Task { ENABLE, DISABLE }

    [SerializeField] private GameObject creation;
    [SerializeField] private Task task;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (this.task == Task.ENABLE)
                this.creation.SetActive(true);
            else if (this.task == Task.DISABLE)
                this.creation.SetActive(false);
        }
    }
}
