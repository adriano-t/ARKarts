using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    Vector3 direction = Vector3.zero;
    float speed = 0;
    float maxSpeed = 0.2f;
    public float turnRadius = 90.0f;
    public float acceleration = 10.0f;
    public float brakePower = 0.5f;
    bool holdLeft = false;
    bool holdRight = false;
    bool holdAccelerator = false;
    bool holdBrake = false;
    
    public Image iconLeft;
    public Image iconRight;
    public Image iconAccelerator;
    public Image iconBrake;
    public Text timeLabel;
    public Text lapLabel;
    float raceTime = 0;
    int targetIndex = 0;
    int lap = 1;
    // Start is called before the first frame update
    bool showed = false;
    public void Start()
    {
        showed = false;
        transform.position = Vector3.one * 10000;
        transform.up = -Camera.main.transform.forward;

    }

    public void ResetPosition()
    {
        lap = 1;
        speed = 0;
        raceTime = 0;
        targetIndex = 0;
        NextTarget();
        if (CatmullRom.instance.controlPointsList.Count > 0)
        {
            Transform t = CatmullRom.instance.controlPointsList[0].transform;
            transform.position = t.position;
            transform.up = t.up;
        }
        else
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 10.0f;
            transform.up = -Camera.main.transform.forward;
        }
    }

    public void Update()
    {
        raceTime += Time.deltaTime; 
        float minutes = Mathf.Floor(raceTime / 60.0f);
        float seconds = Mathf.Floor(Mathf.Repeat(raceTime, 60));
        timeLabel.text = minutes + ":" + seconds;

        //align car to the plane
        if(CatmullRom.instance.controlPointsList.Count > 0)
        {
            if(!showed)
            {
                showed = true;
                ResetPosition();
            }

            Transform t = CatmullRom.instance.controlPointsList[0].transform;
            transform.position = Vector3.ProjectOnPlane(transform.position - t.position,  t.up) + t.position;

            Vector3 forward = transform.forward;
            //Vector3 right = transform.right;
            transform.up = t.up;
            //transform.right = right;
            transform.forward = forward;
            
        }

        /////////////////////
        //controlli manuali
        /////////////////////
        if(holdLeft && Mathf.Abs(speed) > 0.001f)
            transform.RotateAround(transform.position, transform.up, - Time.deltaTime  * turnRadius);

        if(holdRight && Mathf.Abs(speed) > 0.001f)
            transform.RotateAround(transform.position, transform.up, Time.deltaTime  * turnRadius);
            
        if(holdAccelerator)
            speed += acceleration * Time.deltaTime * 0.1f;

        if(holdBrake)
            speed -= acceleration * Time.deltaTime * 0.1f;

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
        Collider[] colls = Physics.OverlapSphere(transform.position, 0.5f);
        foreach(var col in colls)
        {
            if (col.gameObject.CompareTag("target"))
            {
                NextTarget();
            }
            else
            {
                if (speed > maxSpeed / 2.0f)
                    speed *= 0.9f;
            }
        }
    }

    public void NextTarget ()
    {
        targetIndex++;
        var pts = CatmullRom.instance.controlPointsList;
        if (targetIndex > pts.Count - 1)
            targetIndex = 0;

        if (targetIndex == 0 && pts.Count > 3)
        {
            lap++;
            lapLabel.text = "Giri: " + lap;
            if(lap >= 4)
            {
                Debug.Log("fine");
            }
        }

        if (pts.Count > 0)
        {
            foreach(var target in pts)
            {
                target.transform.GetChild(0).gameObject.SetActive(false);
            }

            //enable the target
            pts[targetIndex].transform.GetChild(0).gameObject.SetActive(true);
        }

    }
    public void Accelerate(bool pressed)
    {
        holdAccelerator = pressed;
        iconAccelerator.color = pressed ? Color.white : Color.black;
    }

    public void Brake(bool pressed){
        holdBrake = pressed;
        iconBrake.color = pressed ? Color.white : Color.black;
    }

    public void TurnLeft(bool pressed)
    {
        holdLeft = pressed;
        iconLeft.color = pressed ? Color.white : Color.black;
    }

    public void TurnRight(bool pressed)
    {
        holdRight = pressed;
        iconRight.color = pressed ? Color.white : Color.black;
    }
}
