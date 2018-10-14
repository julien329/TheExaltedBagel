using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDInfos : MonoBehaviour {

    private Text below, above;
    private Text left, right;
    private Controller2D playerController;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start () {
        this.below = GameObject.Find("Below").GetComponent<Text>();
        this.above = GameObject.Find("Above").GetComponent<Text>();
        this.left = GameObject.Find("Left").GetComponent<Text>();
        this.right = GameObject.Find("Right").GetComponent<Text>();

        this.playerController = GameObject.Find("Player").GetComponent<Controller2D>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update () {
        this.below.text = (this.playerController.collisions.below) ? "Below: True" : "Below: False";
        this.above.text = (this.playerController.collisions.above) ? "Above: True" : "Above: False";
        this.left.text = (this.playerController.collisions.left) ? "Left: True" : "Left: False";
        this.right.text = (this.playerController.collisions.right) ? "Right: True" : "Right: False";
    }
}
