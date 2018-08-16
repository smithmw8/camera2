using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

    public float speed;
    public float rotationSpeed;
    public GameObject cube;
    public GameObject cubeSpin;

    void Start ()
    {
       // rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
    {
       
        cube.transform.position = cube.transform.position - Vector3.forward * speed;
         cubeSpin.transform.Rotate(Vector3.right * Time.deltaTime * rotationSpeed);

            // Rotate the object around its local X axis at 1 degree per second
        //transform.Rotate(Vector3.right * Time.deltaTime);

        // ...also rotate around the World's Y axis
      //  transform.Rotate(Vector3.up * Time.deltaTime, Space.World);

       // Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

        //rb.AddForce (movement * speed);
    }
}