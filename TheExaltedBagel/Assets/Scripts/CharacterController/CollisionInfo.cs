using UnityEngine;
using System.Collections;

public class CollisionInfo
{
    public bool isSlippery;
    public bool above, below;
    public bool left, right;
    public float gravityDirection;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void Reset(float gravityDirection)
    {
        this.isSlippery = false;
        this.above = this.below = false;
        this.left = this.right = false;

        this.gravityDirection = gravityDirection;
    }
}