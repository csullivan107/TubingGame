using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float acceleration = 5f;    // Boat acceleration speed
    public float maxSpeed = 15f;       // Max speed the boat can reach
    public float baseTurnSpeed = 50f;  // Base turn speed
    public float waterDragCoefficient = 0.1f;  // Water drag applied to slow the boat
    public float rudderTurnEffect = 0.1f;  // How much the rudder reduces speed during turns
    public float alignmentSpeed = 5f;  // How fast the velocity aligns with the boat's direction
    public float minSpeedThreshold = 0.1f;  // Minimum speed before stopping

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = waterDragCoefficient;  // Apply basic water drag through Rigidbody2D
    }

    void Update()
    {
        HandleMovement();
        HandleTurning();
        EnforceForwardMovement();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");

        if (moveInput != 0)
        {
            // Apply force in the direction the boat is facing (its "forward" direction)
            rb.AddForce(transform.up * moveInput * acceleration);
        }

        // Cap the boat's speed at maxSpeed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        // Stop the boat if it's below the minimum speed threshold and not accelerating
        if (moveInput == 0)
        {
            StopIfBelowThreshold();
        }
    }

    void HandleTurning()
    {
        float turnInput = Input.GetAxis("Horizontal");

        // Reduce velocity based on how much the rudder is turning
        ApplyTurnDrag(turnInput);

        // Apply turning based on speed and turn input
        if (rb.velocity.magnitude > minSpeedThreshold)
        {
            float effectiveTurnSpeed = CalculateTurnSpeed(rb.velocity.magnitude);
            // Apply the rotation based on input and speed
            rb.rotation -= turnInput * effectiveTurnSpeed * Time.deltaTime;
        }
    }

    float CalculateTurnSpeed(float currentSpeed)
    {
        // Calculate the percentage of current speed relative to maxSpeed
        float speedPercentage = currentSpeed / maxSpeed;

        // Apply a smooth gradient to the turn radius: tighter turns at lower speeds
        // For example, turnSpeed will increase as speed decreases, starting from baseTurnSpeed
        float turnSpeedMultiplier = Mathf.Lerp(2.0f, 0.5f, speedPercentage);  // Smooth gradient effect
        return baseTurnSpeed * turnSpeedMultiplier;
    }

    void ApplyTurnDrag(float turnInput)
    {
        // The more the boat turns, the more speed it loses
        float speedReduction = Mathf.Abs(turnInput) * rudderTurnEffect * rb.velocity.magnitude;
        rb.velocity = rb.velocity * (1 - speedReduction * Time.deltaTime);
    }

    void EnforceForwardMovement()
    {
        // Get the current speed and forward direction of the boat
        float speed = rb.velocity.magnitude;
        Vector2 forwardDirection = transform.up;

        // Force the velocity to align with the boat's forward direction, preventing sideways drifting
        rb.velocity = Vector2.Lerp(rb.velocity, forwardDirection * speed, alignmentSpeed * Time.deltaTime);
    }

    void StopIfBelowThreshold()
    {
        if (rb.velocity.magnitude < minSpeedThreshold)
        {
            rb.velocity = Vector2.zero;  // Fully stop the boat if it's barely moving
        }
    }
}
