using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;

    public CollisionInfo collisions;
    public LayerMask collisionMask;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private BoxCollider2D playerCollider;
    private RaycastOrigins raycastOrigins;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start () {
        playerCollider = GetComponent<BoxCollider2D>();
        collisions = new CollisionInfo();
        CalculateRaySpacing();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void Move(Vector3 velocity) {
        // Get new Raycast origins, reset collision infos and save old velocity
        UpdateRaycastOrigins();
        collisions.Reset(velocity);

        // If player moving horizontally, check horizontal collisions
        if (velocity.x != 0) {
            HorizontalCollisions(ref velocity);
        }
        // If player moving horizontally, check vertical collisions
        if (velocity.y != 0) {
            VerticalCollisions(ref velocity);
        }

        // Translate player by the amount specified by the final velocity value
        transform.Translate(velocity);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void HorizontalCollisions(ref Vector3 velocity) {
        // Get direction sign in x axis
        float directionX = Mathf.Sign(velocity.x);
        // Calculate needed rayLength with requested velocity (distance) and skinWitdh
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        // For every horizontal ray...
        for (int i = 0; i < horizontalRayCount; i++) {
            // Find starting point according to direction
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            // Add distance offset beween each ray
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            // Cast ray with collisonMask looking or specific layer
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            // If the Raycast hit something...
            if (hit) {
                // Stop the player from going further than the hit distance
                velocity.x = (hit.distance - skinWidth) * directionX;
                // Set current hit distance as max length for following rays (prevent higher velocity)
                rayLength = hit.distance;              

                // Set collisions bool for left and right
                collisions.left = (directionX == -1);
                collisions.right = (directionX == 1);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void VerticalCollisions(ref Vector3 velocity) {
        // Get direction sign in y axis
        float directionY = Mathf.Sign (velocity.y);
        // Calculate needed rayLength with requested velocity (distance) and skinWitdh
        float rayLength = Mathf.Abs (velocity.y) + skinWidth;

        // For every vertical ray...
		for (int i = 0; i < verticalRayCount; i ++) {
            // Find starting point according to direction
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            // Add distance offset beween each ray
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            // Cast ray with collisonMask looking or specific layer
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            // If the Raycast hit something...
            if (hit) {
                // Stop the player from going further than the hit distance
                velocity.y = (hit.distance - skinWidth) * directionY;
                // Set current hit distance as max length for following rays (prevent higher velocity)
                rayLength = hit.distance;

                // Set collisions bool for above and below
                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void UpdateRaycastOrigins() {
        // Get player collider bounds and substract skinWidth on each side
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(-2 * skinWidth);

        // Set min/max origin points
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void CalculateRaySpacing() {
        // Get player collider bounds and substract skinWidth on each side
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(-2 * skinWidth);

        // Clamp the rayCount with a minimal value
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        // Calculate required spacing between rays
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
