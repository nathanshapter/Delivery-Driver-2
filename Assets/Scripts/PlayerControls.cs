using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] float steerSpeed = 200f;  // Higher for better rotation speed
    [SerializeField] float acceleration = 5f;  // Base force applied for forward movement
    private float originalAcceleration;
    [SerializeField] float deceleration = 2f;  // Speed of deceleration
    [SerializeField] float maxSpeed = 7f;      // Max speed of the car

    [SerializeField] private float currentSpeed = 0f;

    [SerializeField] float[] gearSpeeds = { -2f, 0f, 2f, 4f, 6f, 8f };
    [SerializeField] float[] gearLowestSpeeds;

    public Gear currentGear = Gear.Neutral;

    HUD hud;

    Rigidbody2D rb;

    public float speedInKMH;

    [SerializeField] private float[] gearAccelerationMultipliers = { 0f, 1.5f, 3f, 1.3f, 1.2f, 1.1f };

    private void Start()
    {
        hud = FindObjectOfType<HUD>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        originalAcceleration = acceleration;
    }

    private void Update()
    {



        // Calculate speed in km/h
        float speedInMetersPerSecond = rb.velocity.magnitude;
        speedInKMH = speedInMetersPerSecond * 3.6f;
        currentSpeed = speedInMetersPerSecond;  // Update currentSpeed

        StallCar();

        float steerAmount = Input.GetAxis("Horizontal");
        float movementForward = Input.GetAxis("Vertical");

        // Gear up and down
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentGear < Gear.Fifth)
        {
            if (currentGear == Gear.Neutral)
            {
                currentGear++;
                hud.UpdateGearText(((int)currentGear));
                return;
            }

            if (speedInKMH > gearLowestSpeeds[(int)currentGear] * 0.8f)
            {
                currentGear++;
                hud.UpdateGearText(((int)currentGear));
            }
            else
            {
                print("Go faster to shift up.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && currentGear > Gear.Neutral)
        {
            currentGear--;
            hud.UpdateGearText(((int)currentGear));
        }

        float maxCurrentSpeed = gearSpeeds[(int)currentGear];

        // Calculate adjusted acceleration based on the current gear
        float adjustedAcceleration = acceleration * gearAccelerationMultipliers[(int)currentGear];

        // Apply acceleration or deceleration based on input
        if (movementForward != 0f)  // When player presses W or S
        {
            // Apply force based on input using adjustedAcceleration
            rb.AddForce(transform.up * movementForward * adjustedAcceleration , ForceMode2D.Force);
        }
        else
        {
            // If no input is detected, gradually decelerate
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        // Clamp the velocity to not exceed the max speed for the current gear
        if (rb.velocity.magnitude > maxCurrentSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxCurrentSpeed; // Maintain direction but limit speed
        }

        // Rotate the car based on input
        float rotation = -steerAmount * steerSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation + rotation);

        if (currentGear == Gear.Neutral)
        {
            currentSpeed = 0f;
        }

        CalculateSteerSpeed(speedInKMH);
        ApplyLateralFriction();
    }

    public enum Gear
    {
        Neutral,
        First,
        Second,
        Third,
        Fourth,
        Fifth
    }

    private void StallCar()
    {
        if (currentGear == Gear.First)
        {
            return;
        }
        if (speedInKMH < gearLowestSpeeds[(int)currentGear])
        {
            currentGear = Gear.Neutral;
        }

        hud.UpdateGearText(((int)currentGear));
    }

   float CalculateSteerSpeed(float currentSpeed)
    {
        
        
        float newSteerSpeed = steerSpeed / currentSpeed;
        print(newSteerSpeed);
        
        return newSteerSpeed;
    }


    float lateralFriction = 1f;  // Controls the grip (0 = no grip, 1 = full grip)

    void ApplyLateralFriction()
    {
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right); // Lateral velocity
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up); // Forward velocity

        // Compute the speed factor (higher speeds reduce the grip)
        float speed = rb.velocity.magnitude;
        float gripFactor = Mathf.Clamp01(1f - (speed / maxSpeed));  // Decrease grip at higher speeds

        if (Input.GetKey(KeyCode.LeftShift))
        {
            // While left shift is pressed, allow sliding by reducing lateral friction to a very low value
            lateralFriction = Mathf.Lerp(lateralFriction, 0f, Time.deltaTime * 5f);  // Smoothly reduce friction to 0
        }
        else
        {
            // When left shift is released, smoothly return the lateral friction to normal
            lateralFriction = Mathf.Lerp(lateralFriction, 1f, Time.deltaTime * 5f);  // Smoothly increase friction to normal
        }

        // Apply lateral friction based on the current lateralFriction value
        rb.velocity = forwardVelocity + rightVelocity * lateralFriction * gripFactor;

        // Optional: Add slight drift effect at high speeds
        if (speed > maxSpeed * 0.8f && !Input.GetKey(KeyCode.LeftShift))  // When car is above 80% of max speed, and not drifting
        {
            rb.velocity += rightVelocity * 0.1f;  // Add slight drift effect
        }
    }



}



