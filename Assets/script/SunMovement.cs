using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// build a 2D sun model to attach to the 3D model
public class SunMovement : MonoBehaviour
{
    public Transform sun;
    // set the sun spin speed and position of the sun
    public float spinSpeed;
    public Vector3 sunPosition;

    void Start(){
    }
    // Update the sun position once per frame
    void Update()
    {
        sun.RotateAround(sunPosition, Vector3.right, spinSpeed * Time.deltaTime);
    }
}