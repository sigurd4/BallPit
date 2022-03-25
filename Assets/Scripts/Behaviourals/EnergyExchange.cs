using System.Collections.Generic;
using UnityEngine;
public class EnergyExchange : Behavioural
{
    private readonly int polySize;
    private readonly float charitability;
    
    public EnergyExchange(Ball ball, int polySize, float charitability) : base(ball, 0, 1 + polySize)
    {        
        this.polySize = polySize;
        this.charitability = charitability;
    }
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float energy = this.charitability*(1 - Mathf.Exp(-Time.deltaTime*Neurons.Sigmoid(outputLayer[0])))*fatigueCoefficient*this.ball.energy;
        
        if(energy != 0)
        {
            HashSet<Ball> balls = Ball.GetLiving();
            if(balls.Count != 1)
            {
                float radius = this.ball.radius;
                
                Vector3 force = Vector3.zero;

                float totalStake = 0;
                Dictionary<Ball, float> stake = new Dictionary<Ball, float>();

                foreach(Ball other in balls)
                {
                    if(other == null || other == this.ball) continue;
                    Vector3 R = other.transform.position - this.ball.transform.position;

                    float r = R.magnitude - other.radius - radius;
                    r += Mathf.Sign(r)*0.01f;

                    stake.Add(other, 0.0f);
                    for(int i = 0; i < this.polySize; i++)
                    {
                        float coeff = Neurons.Tanh(outputLayer[1 + i]);
                        for(int j = 0; j <= i; j++)
                        {
                            coeff /= r;
                        }
                        stake[other] += coeff;
                    }
                    stake[other] = Mathf.Abs(stake[other])*other.fatigue;
                    totalStake += stake[other];
                }
                if(totalStake != 0)
                {
                    foreach(Ball other in balls)
                    {
                        if(other == null || other == this.ball) continue;
                        float e = energy*(stake[other]/(totalStake + 0.0001f));
                        e = Mathf.Min(e, other.fatigue);
                        this.ball.AddFatigue(e);
                        other.energy += e;
                    }
                }
            }
        }
    }
}