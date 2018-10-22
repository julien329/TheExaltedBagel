using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDInfos : MonoBehaviour
{
    private Text below, above;
    private Text left, right;
    private Text velocityX, velocityY;
    private Text charges;
    private Controller2D playerCollisions;
    private Player player;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.below = GameObject.Find("Below").GetComponent<Text>();
        this.above = GameObject.Find("Above").GetComponent<Text>();
        this.left = GameObject.Find("Left").GetComponent<Text>();
        this.right = GameObject.Find("Right").GetComponent<Text>();
        this.velocityX = GameObject.Find("VelocityX").GetComponent<Text>();
        this.velocityY = GameObject.Find("VelocityY").GetComponent<Text>();
        this.charges = GameObject.Find("Charges").GetComponent<Text>();

        this.playerCollisions = GameObject.Find("Player").GetComponent<Controller2D>();
        this.player = GameObject.Find("Player").GetComponent<Player>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        this.below.text = (this.playerCollisions.collisions.below) ? "Below: True" : "Below: False";
        this.above.text = (this.playerCollisions.collisions.above) ? "Above: True" : "Above: False";
        this.left.text = (this.playerCollisions.collisions.left) ? "Left: True" : "Left: False";
        this.right.text = (this.playerCollisions.collisions.right) ? "Right: True" : "Right: False";
        this.velocityX.text = "VelocityX: " + this.playerCollisions.collisions.velocity.x.ToString("F1");
        this.velocityY.text = "VelocityY: " +  this.playerCollisions.collisions.velocity.y.ToString("F1");
        this.charges.text = "Charges: " + this.player.gravityChargeCount + " / " + this.player.gravityChargeMax;
    }
}
