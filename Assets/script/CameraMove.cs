using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraMove : MonoBehaviour
{
    public float velocity = 10.0f;
    public float xVelocity = 2.0f;
    public float yVelocity = 2.0f;

    public GameObject terrain;

    public float WaterHeight;

    public float scrollWheelCapture = 0;
    public float sensitivetyMouseWheel = 10f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float[,] heightMap;

    void Start()
    {
        transform.position = new Vector3(30.0f, 70.0f, 30.0f);
        this.GetComponent<Camera>().fieldOfView = 60;

    }
    // Update the scence once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Start();
        }
        heightMap = terrain.GetComponent<TerrainGenerator>().heightMap;
        Mesh mesh = terrain.GetComponent<MeshFilter>().mesh;

        // Make cursor invisible and lock it
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // chagne the angle with the mouse rotation
        yaw += xVelocity * Input.GetAxis("Mouse X");
        pitch -= yVelocity * Input.GetAxis("Mouse Y");

        // zoom in and zoom out using mouse scrollwheel
        if ((Input.GetAxis("Mouse ScrollWheel")  < 0) && this.GetComponent<Camera>().fieldOfView < 105)
        {
            //Debug.Log(scrollWheelCapture);
            this.GetComponent<Camera>().fieldOfView = this.GetComponent<Camera>().fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
            //Debug.Log(this.GetComponent<Camera>().fieldOfView);
        } else if ((Input.GetAxis("Mouse ScrollWheel") > 0) && this.GetComponent<Camera>().fieldOfView > 1)
        {
            this.GetComponent<Camera>().fieldOfView = this.GetComponent<Camera>().fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
            //Debug.Log(this.GetComponent<Camera>().fieldOfView);
        }

        // change orientation of camera
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // vector to store newPosition
        Vector3 newPosition = Vector3.zero;

        // Move object in the direction of camera

        // move to forward direction
        if (Input.GetKey(KeyCode.W))
        {
            newPosition += Camera.main.transform.forward * velocity * Time.deltaTime;
        }
        // move to backward direction
        if (Input.GetKey(KeyCode.S))
        {
            newPosition += Camera.main.transform.forward * -velocity * Time.deltaTime;
        }
        // move to left direction
        if (Input.GetKey(KeyCode.A))
        {
            newPosition += Camera.main.transform.right * -velocity * Time.deltaTime;
        }
        // move to right direction
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += Camera.main.transform.right * velocity * Time.deltaTime;
        }
        // check if the camera reaches the boundries
        if (transform.position.x + newPosition.x <= mesh.bounds.size.x - 1 && transform.position.x + newPosition.x >= 1 &&
            transform.position.z + newPosition.z <= mesh.bounds.size.z - 1 && transform.position.z + newPosition.z >= 1 &&
            transform.position.y + newPosition.y > WaterHeight + 1)
        {
            // move camera to new position and stop it when it hits the mountains or sea
            if (newPosition.y < 0 && 
                (transform.position.y + newPosition.y) - 3 > heightMap[(int)(transform.position.x + newPosition.x), (int)(transform.position.z + newPosition.z)]){
                transform.position += newPosition;
            } else if (newPosition.y > 0 && 
                (transform.position.y + newPosition.y) - 3 > heightMap[(int)(transform.position.x + newPosition.x), (int)(transform.position.z + newPosition.z)]){
                transform.position += newPosition;
            }
        }

    }
}