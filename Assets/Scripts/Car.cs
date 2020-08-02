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
        /////////////////////
        //controlli manuali
        /////////////////////
        if(holdLeft && Mathf.Abs(speed) > 0.001f)
            transform.RotateAround(transform.position, transform.up, - Time.deltaTime  * turnRadius);

        if(holdRight && Mathf.Abs(speed) > 0.001f)
            transform.RotateAround(transform.position, transform.up, Time.deltaTime  * turnRadius);
            
        if(holdAccelerator)
            speed += acceleration * Time.deltaTime * 0.01f;

        if(holdBrake)
            speed -= acceleration * Time.deltaTime * 0.01f;

        //limita la velocita' massima
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        //rallenta
        speed *= 0.99f;
        if(speed < 0.01f)
            speed *= 0.1f;

        //rallenta di piu' se gira
        if(holdLeft || holdRight) 
            speed *= 0.99f;

        transform.position -= transform.forward * speed;
        //Debug.Log("speed: " + speed);

        //limita la posizione della macchina a un certo raggio intorno alla pista
        transform.position = Vector3.ClampMagnitude(transform.position, 10.0f);

        //slow down outside
        if(!Physics.CheckSphere(transform.position, 1.2f))
        {
            if(speed > maxSpeed/2.0f)
                speed *= 0.9f;
        }

        //align car to the plane
        if(CatmullRom.instance.controlPointsList.Count > 0)
        {
            Transform t = CatmullRom.instance.controlPointsList[0].transform;
            transform.position = Vector3.ProjectOnPlane(transform.position - t.position,  t.up) + t.position;
            transform.up = t.up;
        }
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
