using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour {

    [SerializeField] private bool clicked;
    [SerializeField] private Material allClicked;
    [SerializeField] private Material notAllClicked;
    private Transform button;
    private Vector3 unclickedPos;
    private Vector3 clickedPos;

    private void Awake()
    {
        this.button = this.transform.Find("Button");
        Transform buttonBase = this.transform.Find("ButtonBase");
        unclickedPos = button.transform.position;
        if (buttonBase.position.y - this.button.position.y < -0.01f)
            clickedPos = new Vector3(this.button.transform.position.x, this.button.transform.position.y - 0.15f, this.button.transform.position.z);
        else if (buttonBase.position.y - this.button.position.y > 0.01f)
            clickedPos = new Vector3(this.button.transform.position.x, this.button.transform.position.y + 0.15f, this.button.transform.position.z);
        else if (buttonBase.position.x - this.button.position.x < -0.01f)
            clickedPos = new Vector3(this.button.transform.position.x - 0.15f, this.button.transform.position.y, this.button.transform.position.z);
        else if (buttonBase.position.x - this.button.position.x > 0.01f)
            clickedPos = new Vector3(this.button.transform.position.x + 0.15f, this.button.transform.position.y, this.button.transform.position.z);

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && !this.clicked)
        {
            this.button.transform.position = clickedPos;
            this.clicked = true;
            this.GetComponent<AudioSource>().Play();
        }
    }

    public void ResetButton()
    {
        this.button.transform.position = unclickedPos;
        this.transform.Find("Button").GetComponent<MeshRenderer>().material = notAllClicked;
        clicked = false;
    }

    public void AllClicked()
    {
        this.transform.Find("Button").GetComponent<MeshRenderer>().material = allClicked;
    }

    public bool GetClicked()
    {
        return clicked;
    }
}
