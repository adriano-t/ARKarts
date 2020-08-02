using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Vector3.ProjectOnPlane - example

// Generate a random plane in xy. Show the position of a random
// vector and a connection to the plane. The example shows nothing
// in the Game view but uses Update(). The script reference example
// uses Gizmos to show the positions and axes in the Scene.

public class TestPlane : MonoBehaviour
{
    public Transform[] cubes; 

    // Generate the values for all the examples.
    // Change the example every two seconds.
    void Update ()
    { 
            for (var i = 0; i < cubes.Length; i++)
            {
                Vector3 pos = Vector3.ProjectOnPlane(cubes[i].transform.position - transform.position, transform.up) + transform.position;
                cubes[i].transform.up = transform.up;
                cubes[i].transform.position = pos;
            }
    }

   
}