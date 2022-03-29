using System;
using UnityEngine;
public class GravityManipulator : Behavioural
{
    private readonly float sens;
    public GravityManipulator(Ball ball, float sens) : base(ball, 0, 6)
    {
        this.sens = sens;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        Vector3 dir = new Vector3(Mathf.Sin(Neurons.Finite(sens*Mathf.Pow(outputLayer[0], outputLayer[1]))), Mathf.Sin(Neurons.Finite(sens*Mathf.Pow(outputLayer[2], outputLayer[3]))), Mathf.Sin(Neurons.Finite(sens*Mathf.Pow(outputLayer[4], outputLayer[5]))));

        this.ball.rigidbody.AddForce(dir.normalized*Utils.earthGravityAcceleration*this.ball.mass);
    }

    public override Behavioural Clone(Ball ball)
    {
        return new GravityManipulator(ball, this.sens);
    }
}