using UnityEngine;
using System.Collections;

public class CollisionInfo {

    public bool above, below;
    public bool left, right;
    public float gravityDirection;
    public Vector2 velocity;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void Reset(float gravityDirection) {
        this.above = this.below = false;
        this.left = this.right = false;

        this.gravityDirection = gravityDirection;
        this.velocity = Vector2.zero;
    }
}