using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb {

    Component[] joints;
    HingeJoint bodyConnection;
    HingeJoint lowerLegConnection;

    float force = 10;

    public Limb(GameObject thigh, GameObject body)
    {
        joints = thigh.GetComponents(typeof(HingeJoint));

        foreach(HingeJoint joint in joints)
        {
            if(joint.connectedBody == body.GetComponent<Rigidbody>())
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

    public void addForceToBodyHinge(float targetVelocity)
    {
        JointMotor motor = bodyConnection.motor;
        motor.targetVelocity = targetVelocity * 500;
        motor.force = force;
        bodyConnection.motor = motor;
        bodyConnection.useMotor = true;
    }

    public void addForceToLegHinge(float targetVelocity)
    {
        JointMotor motor = lowerLegConnection.motor;
        motor.targetVelocity = targetVelocity * 100;
        motor.force = force;
        lowerLegConnection.motor = motor;
        lowerLegConnection.useMotor = true;
    }
}
