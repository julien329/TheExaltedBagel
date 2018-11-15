using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class Controller2D : MonoBehaviour
{
    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private CollisionInfo collisions;
    private BoxCollider playerCollider;
    private RaycastOrigins raycastOrigins;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public CollisionInfo Collisions
    {
        get { return this.collisions; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.playerCollider = GetComponent<BoxCollider>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void Initialize()
    {   
        this.collisions = new CollisionInfo();
        CalculateRaySpacing();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void Move(Vector3 velocity, float gravityDirection)
    {
        // Get new Raycast origins, reset collision infos and save old velocity
        UpdateRaycastOrigins();
        ResetCollisionsInfo();

        // If player moving horizontally, check horizontal collisions
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        // If player moving horizontally, check vertical collisions
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity, gravityDirection);
        }

        // Translate player by the amount specified by the final velocity value
        this.transform.Translate(velocity);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void HorizontalCollisions(ref Vector3 velocity)
    {
        // Get direction sign in x axis
        float directionX = Mathf.Sign(velocity.x);
        // Calculate needed rayLength with requested velocity (distance) and skinWitdh
        float rayLength = Mathf.Abs(velocity.x) + this.skinWidth;

        // For every horizontal ray...
        for (int i = 0; i < this.horizontalRayCount; ++i)
        {
            // Find starting point according to direction
            Vector3 rayOrigin = (directionX == -1) ? this.raycastOrigins.bottomLeft : this.raycastOrigins.bottomRight;
            // Add distance offset beween each ray
            rayOrigin += Vector3.up * (this.horizontalRaySpacing * i);

            // Cast ray with collisonMask looking or specific layer
            RaycastHit hit;
            // If the Raycast hit something...
            if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, this.collisionMask))
            {
                // Stop the player from going further than the hit distance
                velocity.x = (hit.distance - this.skinWidth) * directionX;
                // Set current hit distance as max length for following rays (prevent higher velocity)
                rayLength = hit.distance;

                // Set collisions bool for left and right
                this.collisions.left = (directionX == -1);
                this.collisions.right = (directionX == 1);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void VerticalCollisions(ref Vector3 velocity, float gravityDirection)
    {
        // Get direction sign in y axis
        float directionY = Mathf.Sign(velocity.y);
        // Calculate needed rayLength with requested velocity (distance) and skinWitdh
        float rayLength = Mathf.Abs(velocity.y) + this.skinWidth;

        // For every vertical ray...
		for (int i = 0; i < this.verticalRayCount; ++i)
        {
            // Find starting point according to direction
            Vector3 rayOrigin = (directionY == -1) ? this.raycastOrigins.bottomLeft : this.raycastOrigins.topLeft;
            // Add distance offset beween each ray
            rayOrigin += Vector3.right * (this.verticalRaySpacing * i + velocity.x);

            Debug.DrawRay(rayOrigin, Vector3.up * directionY * 1, Color.red);

            // Cast ray with collisonMask looking or specific layer
            RaycastHit hit;
            // If the Raycast hit something...
            if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, this.collisionMask))
            {
                // Stop the player from going further than the hit distance
                velocity.y = (hit.distance - this.skinWidth) * directionY;
                // Set current hit distance as max length for following rays (prevent higher velocity)
                rayLength = hit.distance;

                // Set collisions bool for above and below
                this.collisions.below = (directionY * gravityDirection == -1);
                this.collisions.above = (directionY * gravityDirection == 1);

                // Set surface type under the middle ray only
                if ((i + 1) == Mathf.CeilToInt(this.verticalRayCount / 2f))
                { 
                    this.collisions.isSlippery = this.collisions.below && hit.collider.CompareTag("Ice");
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void UpdateRaycastOrigins()
    {
        // Get player collider bounds and substract skinWidth on each side
        Bounds bounds = this.playerCollider.bounds;
        bounds.Expand(-2 * this.skinWidth);

        // Set min/max origin points
        this.raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        this.raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        this.raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        this.raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void CalculateRaySpacing()
    {
        // Get player collider bounds and substract skinWidth on each side
        Bounds bounds = this.playerCollider.bounds;
        bounds.Expand(-2 * this.skinWidth);

        // Clamp the rayCount with a minimal value
        this.horizontalRayCount = Mathf.Clamp(this.horizontalRayCount, 2, int.MaxValue);
        this.verticalRayCount = Mathf.Clamp(this.verticalRayCount, 2, int.MaxValue);

        // Calculate required spacing between rays
        this.horizontalRaySpacing = bounds.size.y / (this.horizontalRayCount - 1);
        this.verticalRaySpacing = bounds.size.x / (this.verticalRayCount - 1);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void ResetCollisionsInfo()
    {
        this.collisions.isSlippery = false;
        this.collisions.above = this.collisions.below = false;
        this.collisions.left = this.collisions.right = false;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public struct CollisionInfo
    {
        public bool isSlippery;
        public bool above, below;
        public bool left, right;
    }
}
