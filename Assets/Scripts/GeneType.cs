/* creating a class that will act as a Gene in my algorithm
 * Author: Ka Yu Chan
 * Date: 30/10/2017
 */

using UnityEngine;

public class GeneType
{
    private System.Random generator = new System.Random();
    public float targetVelocity { get; private set; }
    public int jLimitMax { get; private set; }
    public int jLimitMin { get; private set; }
    public float velocity { get; private set; }
    public Vector3 newAxis { get; private set; }


    public GeneType(int jointLimit, float velocity)
    {
        //Setting up variables
        this.velocity = velocity;

        jLimitMax = Random.Range(jointLimit, 0);
        jLimitMin = Random.Range(0, -jointLimit);

        targetVelocity = Random.value > 0.5 ? velocity : -velocity;
        newAxis = new Vector3(Random.Range(1,10), Random.Range(1, 10), Random.Range(1, 10));
    }
}