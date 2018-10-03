using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDInfos : MonoBehaviour {

    private Text below, above;
    private Text left, right;
    private Controller2D playerController;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start () {
        below = GameObject.Find("Below").GetComponent<Text>();
        above = GameObject.Find("Above").GetComponent<Text>();
        left = GameObject.Find("Left").GetComponent<Text>();
        right = GameObject.Find("Right").GetComponent<Text>();

        playerController = GameObject.Find("Player").GetComponent<Controller2D>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update () {
        below.text = (playerController.collisions.below) ? "Below: True" : "Below: False";
        above.text = (playerController.collisions.above) ? "Above: True" : "Above: False";
        left.text = (playerController.collisions.left) ? "Left: True" : "Left: False";
        right.text = (playerController.collisions.right) ? "Right: True" : "Right: False";
    }
}
