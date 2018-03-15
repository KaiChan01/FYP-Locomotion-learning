using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollision : MonoBehaviour {

    // Use this for initialization

    private bool isTouching;
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Ground" )
        {
            isTouching = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Ground")
        {
            isTouching = false;
        }
    }

    public bool isTouchingGround()
    {
        return isTouching;
    }
}
