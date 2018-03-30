using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is a collision detection script for detecting if the attached gameobject is touching the ground 
public class BodyCollision : MonoBehaviour {

    private bool isTouching;

    void Start () {
        isTouching = false;
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
