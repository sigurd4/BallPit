using UnityEngine;
using System;
public class SelfDigester : Behavioural
{
    private readonly float digestion;
    public SelfDigester(Ball ball, float digestion) : base(ball, 0, 3)
    {
        this.digestion = digestion;
    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        if(fatigueCoefficient != 1f)
        {
            float mass = this.ball.rigidbody.mass;

            float fatigue = this.ball.fatigue;
            
            float x = (1 - Mathf.Exp(-this.digestion*fatigue*Time.deltaTime*Neurons.Sigmoid(outputLayer[0])))*Neurons.Sigmoid(outputLayer[1] - fatigueCoefficient);

            x = Neurons.Tanh(Mathf.Pow(x, Neurons.Tanh(outputLayer[2])));

            if(x > 0)
            {
                float fatigueRecovery = fatigue*x;
                if(this.BurnMass(fatigueRecovery))
                {
                    fatigue -= fatigueRecovery;
                }
                this.ball.fatigue = fatigue;
            }
        }
    }
    
    private bool BurnMass(float energy)
    {
        if(energy == 0 || Single.IsNaN(energy)) return false;

        float mass = ball.mass;
        float dmass = energy/Utils.speedOfLightSquared;
        if(dmass >= mass)
        {
            this.ball.Kill("Heat death");
            return false;
        }
        ball.mass = mass - dmass;
        return true;
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new SelfDigester(ball, this.digestion);
    }
}