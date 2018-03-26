using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb {

    Component[] joints;
    int jointNum;
    /*
    HingeJoint bodyConnection;
    HingeJoint lowerLegConnection;
    */

    float force = 10;

    public Limb(GameObject jointComponent, GameObject body)
    {
        joints = jointComponent.GetComponents(typeof(HingeJoint));

        jointNum = joints.Length;
        /*
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
        */
    }

    public float getJointAngle(int jointIndex)
    {
        HingeJoint tempJoint = (HingeJoint)joints[jointIndex];
        return tempJoint.angle;
    }

    public int getJointNum()
    {
        return jointNum;
    }

    public void addForceToHinge(float targetVelocity, int jointIndex)
    {
        HingeJoint tempJoint = (HingeJoint) joints[jointIndex];
        JointMotor motor = tempJoint.motor;
        motor.targetVelocity = targetVelocity * 1000;
        motor.force = force;
        tempJoint.motor = motor;
        tempJoint.useMotor = true;
    }
}
