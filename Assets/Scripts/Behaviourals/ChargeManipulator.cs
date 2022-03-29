using UnityEngine;
using System.Collections.Generic;
using System;
public class ChargeManipulator : Behavioural
{
    private readonly float chargeDensityMax;
    private readonly int order;
    public ChargeManipulator(Ball ball, int order, float chargeDensityMax) : base(ball, 0, 2*order)
    {
        this.order = order;
        this.chargeDensityMax = chargeDensityMax;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        if(fatigueCoefficient != 0f)
        {
            float mass = this.ball.mass;
            float energy = 0f;

            this.ball.charge = new float[this.order];

            for(int i = 0; i < this.order; i++)
            {
                float charge = chargeDensityMax*mass*Neurons.Tanh(Mathf.Sign(outputLayer[2*i])*Mathf.Pow(Mathf.Abs(Neurons.Tanh(outputLayer[2*i])), Neurons.Tanh(outputLayer[2*i + 1])))*fatigueCoefficient;
                
                if(!Single.IsNaN(charge))
                {
                    float radius = Mathf.Pow(this.ball.radius, i + 1);
                    energy += Mathf.Abs(Utils.k*charge*charge*charge/radius);

                    this.ball.charge[i] = charge;
                }
            }

            this.ball.AddFatigue(energy);
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new ChargeManipulator(ball, this.order, this.chargeDensityMax);
    }
}