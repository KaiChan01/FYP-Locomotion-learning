using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb {

    //Stores all of the HingeJoint components in the gameobject
    Component[] joints;
    int jointNum;

    public Limb(GameObject jointComponent)
    {
        joints = jointComponent.GetComponents(typeof(HingeJoint));

        jointNum = joints.Length;
    }

    //Get the anlge of the joint given the index of the jointArray
    public float getJointAngle(int jointIndex)
    {
        HingeJoint tempJoint = (HingeJoint)joints[jointIndex];
        return tempJoint.angle;
    }

    //Return the number of hinge joints in this limb
    public int getJointNum()
    {
        return jointNum;
    }

    //Add force to limb
    public void addForceToHinge(float targetVelocity, int jointIndex)
    {
        HingeJoint tempJoint = (HingeJoint) joints[jointIndex];
        JointMotor motor = tempJoint.motor;
        motor.targetVelocity = targetVelocity * 1000;
        motor.force = Mathf.Abs(targetVelocity) * 1000;
        tempJoint.motor = motor;
        tempJoint.useMotor = true;
    }

    public void revertToOriginalPosition()
    {
        for(int i = 0; i < jointNum; i++)
        {
            HingeJoint tempJoint = (HingeJoint)joints[i];
            JointMotor motor = tempJoint.motor;
            tempJoint.useSpring = true;
            tempJoint.useMotor = false;
        }
    }
}
