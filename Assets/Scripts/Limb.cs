using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb {

    Component[] joints;
    HingeJoint bodyConnection;
    HingeJoint lowerLegConnection;

    public Limb(GameObject thigh, GameObject body)
    {
        joints = thigh.GetComponents(typeof(HingeJoint));

        foreach(HingeJoint joint in joints)
        {
            if(joint.connectedBody = body.GetComponent<Rigidbody>())
            {
                bodyConnection = joint;
            }
            else
            {
                lowerLegConnection = joint;
            }
        }
    }

    public float getBodyConnAngle()
    {
        return bodyConnection.angle;
    }

    public float getLegConnAngle()
    {
        return lowerLegConnection.angle;
    }
}
