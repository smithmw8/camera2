using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Main : MonoBehaviour {


	public GameObject sphere; 
	public float rotationSpeed;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
		// Rotate the object around its local X axis at 1 degree per second
        sphere.transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);

	}
}
