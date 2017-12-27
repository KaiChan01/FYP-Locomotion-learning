using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb {

    Component[] joints;
    HingeJoint bodyConnection;
    HingeJoint lowerLegConnection;
    float targetVelocity = 50;

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

    public void addForceToBodyHinge(float force)
    {
        JointMotor motor = bodyConnection.motor;
        motor.targetVelocity = targetVelocity;
        motor.force = force*1000;
        bodyConnection.motor = motor;
        bodyConnection.useMotor = true;
    }

    public void addForceToLegHinge(float force)
    {
        JointMotor motor = lowerLegConnection.motor;
        motor.targetVelocity = targetVelocity;
        motor.force = force*1000;
        lowerLegConnection.motor = motor;
        lowerLegConnection.useMotor = true;
    }
}
