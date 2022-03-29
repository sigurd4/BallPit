using UnityEngine;
using System;
public class TorqueEngine : Behavioural
{
    private readonly float torque;
    public TorqueEngine(Ball ball, float torque) : base(ball, 1, 5)
    {
        this.torque = torque;
    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        if(fatigueCoefficient != 0f)
        {
            Vector3 torqueActuator = new Vector3(
                Neurons.Tanh(outputLayer[0]),
                Neurons.Tanh(outputLayer[1]),
                Neurons.Tanh(outputLayer[2])
            )*Neurons.ReLU(outputLayer[3]);

            torqueActuator = torqueActuator.normalized*Mathf.Pow(torqueActuator.magnitude, Neurons.Sigmoid(outputLayer[4]));

            inputLayer[0] = torqueActuator.magnitude;

            Vector3 torque = torqueActuator*this.torque*this.ball.mass*fatigueCoefficient*Time.deltaTime;

            if(!Utils.IsFinite(torque))
            {
                return;
            }

            this.ball.rigidbody.AddRelativeTorque(torque);

            Vector3 inertia = this.ball.inertia;
            Vector3 inertiaInv = inertia;

            this.ball.AddFatigue(Vector3.Dot(torque, (Vector3.Cross(torque, inertia.normalized)/inertia.magnitude))*Time.deltaTime);
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new TorqueEngine(ball, this.torque);
    }
}