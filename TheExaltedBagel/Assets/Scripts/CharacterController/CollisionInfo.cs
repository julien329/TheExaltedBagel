using UnityEngine;
using System.Collections;

public class CollisionInfo {

    public bool above, below;
    public bool left, right;
    public Vector3 velocityOld;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void Reset(Vector3 velocity) {
        above = below = false;
        left = right = false;

        velocityOld = velocity;
    }
}
