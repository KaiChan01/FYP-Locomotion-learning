using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBalance : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float imbalanceMeasure =
            (Mathf.Abs(transform.rotation.eulerAngles.x) +
            Mathf.Abs(transform.rotation.eulerAngles.y) +
            Mathf.Abs(transform.rotation.eulerAngles.z)) * 0.01f;

        float eulerX = transform.rotation.eulerAngles.x % 360;
        if( eulerX > 270 )
        {
            eulerX = eulerX - 360;
        }

        Debug.Log(imbalanceMeasure);
        Debug.Log(eulerX);
        Debug.Log(transform.rotation.x);
    }
}
