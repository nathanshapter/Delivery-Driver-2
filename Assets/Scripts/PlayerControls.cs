using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] float steerSpeed = 0.1f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float deceleration = 1f;
    [SerializeField] float maxSpeed = 7f;

   [SerializeField] private float currentSpeed = 0f;

    [SerializeField] float[] gearSpeeds = { -2f, 0f, 2f, 4f, 6f, 8f };
    [SerializeField] float[] gearLowestSpeeds;

    [SerializeField] private Gear currentGear = Gear.Neutral;

    bool carHasStarted, carHasStalled;

  [SerializeField]  float maxCurrentSpeed;
    [SerializeField] float lowestCurrentSpeed;

    private void Update()
    {

        StallCar(); 

        float steerAmount = Input.GetAxis("Horizontal");
        float movementForward = Input.GetAxis("Vertical");



        if (Input.GetKeyDown(KeyCode.UpArrow) && currentGear < Gear.Fifth)
        {

            if (currentGear == Gear.Neutral)
            {
                currentGear++;
                return;
            }


            if (currentSpeed > gearSpeeds[(int) currentGear] * 0.8f)
            {
                currentGear++;
            }
            else
            {
                print("go faster ");
            }

            
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow) && currentGear > Gear.Neutral)
        {
            currentGear--;
        }


        maxCurrentSpeed = gearSpeeds[(int) currentGear];
        lowestCurrentSpeed = gearLowestSpeeds[(int) currentGear];

        // Apply acceleration or deceleration based on input
        if (movementForward != 0f)  // When player presses W or S
        {
            // Accelerate based on input
            currentSpeed += movementForward * acceleration * Time.deltaTime;
        }
        else
        {
            // If no input is detected, immediately start decelerating
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -gearSpeeds[(int)currentGear], maxCurrentSpeed);


        //rotates the vehicle
        transform.Rotate(0, 0, -steerAmount * steerSpeed );
        // moves the vehicle forward and back
        transform.Translate(0, currentSpeed * Time.deltaTime * moveSpeed, 0);

       
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
        if (currentSpeed < lowestCurrentSpeed)
        {
            currentGear = Gear.Neutral;
        }

        

        
    } 
    public float CurrentSpeedInKM()
    {
        float currentSpeedInKM = currentSpeed * 18;
        return currentSpeedInKM;
    }
 
}
