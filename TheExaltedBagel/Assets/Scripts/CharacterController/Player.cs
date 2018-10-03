using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider2D))]
public class Player : MonoBehaviour {

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float accelerationTimeAirborne = 0.2f;
    [SerializeField] private float accelerationTimeGrounded = 0.1f;

    private float gravity;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private Vector3 velocity;
    private Controller2D controller;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start () {
        controller = GetComponent<Controller2D>();

        // Formula : deltaMovement = velocityInitial * time + (acceleration * time^2) / 2  -->  where acceleration = gravity and velocityInitial is null
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        // Formula : velocityFinal = velocityInitial + acceleration * time  -->  where velocityFinal = jumpVelocity and velocityInitial is null
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update () {
        // Get player input in raw states
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Check horizontal and vertical movements
        MoveH(input);
        MoveV(input);

        // Call move to check collisions and translate the player
        controller.Move(velocity * Time.deltaTime);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveH(Vector2 input) {
        // Set target velocity according to user input
        float targetVelocityX = input.x * moveSpeed;
        // Smooth velocity (use acceleration). Change smoothing value if grounded or airborne
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        // If speed too small, set to null
        if (Mathf.Abs(velocity.x) < 0.1f) {
            velocity.x = 0f;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveV(Vector2 input) {
        // If there is a collision in Y axis, reset velocity
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        // If the jump key is pressed
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            velocity.y = jumpVelocity;
        }

        // Add gravity force downward to Y velocity
        velocity.y += gravity * Time.deltaTime;
    }
}