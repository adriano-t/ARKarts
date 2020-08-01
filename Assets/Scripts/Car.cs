using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    Vector3 direction = Vector3.zero;
    float speed = 0;
    float maxSpeed = 0.1f;
    public float turnRadius = 90.0f;
    public float acceleration = 10.0f;
    public float brakePower = 0.5f;
    bool holdLeft = false;
    bool holdRight = false;
    bool holdAccelerator = false;
    bool holdBrake = false;
    
    
    // Start is called before the first frame update
    public void Start()
    {
        
    }

    public void Update()
    {
        if(holdLeft)
            transform.RotateAround(transform.position, transform.up, - Time.deltaTime * turnRadius * speed/maxSpeed);

        if(holdRight)
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * turnRadius* speed/maxSpeed);
            
        if(holdAccelerator)
            speed += acceleration * Time.deltaTime * 0.01f;

        if(holdBrake)
            speed -= acceleration * Time.deltaTime * 0.01f;

        //limita la velocita' massima
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        //rallenta
        speed *= 0.999f;

        //rallenta di piu' se gira
        if(holdLeft || holdRight) 
            speed *= 0.999f;

        transform.position -= transform.forward * speed;
        Debug.Log("speed: " + speed);
    }

    public void Accelerate(bool pressed)
    {
        holdAccelerator = pressed;
    }

    public void Brake(bool pressed){
        holdBrake = pressed;
    }

    public void TurnLeft(bool pressed)
    {
        holdLeft = pressed;
    }

    public void TurnRight(bool pressed)
    {
        holdRight = pressed;
    }
}
