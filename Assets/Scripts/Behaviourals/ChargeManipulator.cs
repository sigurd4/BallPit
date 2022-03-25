using UnityEngine;
using System.Collections.Generic;
using System;
public class ChargeManipulator : Behavioural
{
    public float power;
    public ChargeManipulator(Ball ball, float power) : base(ball, 0, 1)
    {
        this.power = power;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float mass = this.ball.mass;

        float charge = Neurons.Tanh(outputLayer[0])*this.power*fatigueCoefficient*mass;
        
        if(!Single.IsNaN(charge))
        {
            float radius = this.ball.radius;
            float energy = Mathf.Abs(Utils.k*(Mathf.Pow(charge, 3) - Mathf.Pow(this.ball.charge, 3))/radius);

            this.ball.charge = charge;

            this.ball.AddFatigue(energy);
        }
    }
}