using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class for moving the camera and navigating the simulation
public class CameraController : MonoBehaviour {

    public GameObject camera;

    // Use this for initialization
    void Start () {
        if (camera == null)
        {
            camera = Camera.main.gameObject;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("a"))
        {
            transform.position += camera.transform.right * -10f;
        }

        if (Input.GetKeyDown("d"))
        {
            transform.position += camera.transform.right * 10f;
        }
    }
}
