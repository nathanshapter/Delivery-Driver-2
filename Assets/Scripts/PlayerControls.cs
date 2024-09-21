using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] float steerSpeed = 200f;  // Higher for better rotation speed
    [SerializeField] float acceleration = 5f;  // Force applied for forward movement
    [SerializeField] float deceleration = 2f;  // Speed of deceleration
    [SerializeField] float maxSpeed = 7f;      // Max speed of the car

    [SerializeField] private float currentSpeed = 0f;

    [SerializeField] float[] gearSpeeds = { -2f, 0f, 2f, 4f, 6f, 8f };
    [SerializeField] float[] gearLowestSpeeds;

    public Gear currentGear = Gear.Neutral;

    HUD hud;

    Rigidbody2D rb;

    public float speedInKMH;

    private void Start()
    {
        hud = FindObjectOfType<HUD>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
    }

    private void Update()
    {

   

        // Calculate speed in km/h
        float speedInMetersPerSecond = rb.velocity.magnitude;
        speedInKMH = speedInMetersPerSecond * 3.6f;
       

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

            if (speedInKMH > gearSpeeds[(int)currentGear] * 0.8f)
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

        // Apply acceleration or deceleration based on input
        if (movementForward != 0f)  // When player presses W or S
        {
            // Apply force based on input
            rb.AddForce(transform.up * movementForward * acceleration, ForceMode2D.Force);
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
        if (rb.velocity.magnitude < gearLowestSpeeds[(int)currentGear])
        {
            currentGear = Gear.Neutral;
        }

        hud.UpdateGearText(((int)currentGear));
    }
}
