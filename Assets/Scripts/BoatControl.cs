using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float acceleration = 5f;  // Boat acceleration speed
    public float maxSpeed = 10f;     // Max speed the boat can reach
    public float turnSpeed = 200f;   // Turn speed

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Handle boat throttle
        float moveInput = Input.GetAxis("Vertical");
        rb.AddForce(transform.up * moveInput * acceleration);

        // Limit boat speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        // Handle turning
        float turnInput = Input.GetAxis("Horizontal");
        rb.rotation -= turnInput * turnSpeed * Time.deltaTime;
    }
}
