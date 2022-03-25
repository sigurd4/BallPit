using UnityEngine;
using System;
public class TorqueEngine : Behavioural
{
    private readonly float torque;

    public TorqueEngine(Ball ball, float torque) : base(ball, 1, 4)
    {
        this.torque = torque;
    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float mass = this.ball.mass;

        Vector3 torqueActuator = new Vector3(
            Neurons.Tanh(outputLayer[0]),
            Neurons.Tanh(outputLayer[1]),
            Neurons.Tanh(outputLayer[2])
        )*Neurons.ReLU(outputLayer[3]);

        inputLayer[0] = torqueActuator.magnitude;

        Vector3 torque = torqueActuator*(this.torque*fatigueCoefficient*mass*Time.deltaTime);

        if(!Utils.IsFinite(torque))
        {
            return;
        }

        this.ball.rigidbody.AddRelativeTorque(torque);

        float inertia = this.ball.rigidbody.inertiaTensor.magnitude;

        this.ball.AddFatigue(torque.magnitude*torque.magnitude/inertia*Time.deltaTime);
    }
}