using UnityEngine;
using System;
public class Wiggler : Behavioural
{
    private readonly float wiggle;
    public Wiggler(Ball ball, float wiggle) : base(ball, 0, 3)
    {
        this.wiggle = wiggle;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        if(fatigueCoefficient != 0)
        {
            float actuator = Neurons.Tanh(Mathf.Pow(Neurons.Sigmoid(outputLayer[0]), Neurons.Tanh(outputLayer[1])));

            float mass = this.ball.mass;
            
            float x = Mathf.Pow(Utils.GetRandomPositiveScalar(BallPit.rand), Neurons.Sigmoid(outputLayer[2]));

            float forceM = x*actuator*this.wiggle*Time.deltaTime*fatigueCoefficient;
            if(Single.IsInfinity(forceM) || Single.IsNaN(forceM))
            {
                return;
            }
            
            Vector3 dir = Utils.GetRandomDirection(BallPit.rand)*this.ball.transform.up;

            Vector3 force = forceM*dir;
            
            this.ball.rigidbody.AddForce(force);

            this.ball.AddFatigue(forceM*forceM/mass*Time.timeScale);
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new Wiggler(ball, this.wiggle);
    }
}