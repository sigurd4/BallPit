using UnityEngine;
using System;
using System.Collections.Generic;
public class Ears : Behavioural
{
    int listeningComplexity;
    float maxRadius;
    public Ears(Ball ball, int listeningComplexity, float maxRadius) : base(ball, listeningComplexity*2, listeningComplexity*2)
    {
        this.listeningComplexity = listeningComplexity;
        this.maxRadius = maxRadius;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        Vector3 earPos = this.ball.transform.right*this.ball.radius;

        HashSet<Ball> balls = Ball.GetLiving();
        Vector2[] listening = new Vector2[this.listeningComplexity];
        foreach(Ball other in balls)
        {
            if(other == null) continue;
            Vector3 r = other.transform.position + other.transform.forward*other.radius - this.ball.transform.position;

            float cosear = Vector2.Dot(this.ball.transform.right, r.normalized);

            Vector2 m = new Vector2(0.1f + 0.9f*Neurons.Sigmoid(cosear), 0.1f + 0.9f*Neurons.Sigmoid(-cosear));

            for(int i = 0, l = Math.Min(other.voice.Length, this.listeningComplexity); i < l; i++)
            {
                listening[i] += m*new Vector2(other.voice[i]/Mathf.Pow((r - earPos).magnitude, 2), other.voice[i]/Mathf.Pow((r + earPos).magnitude, 2));
            }
        }
        for(int i = 0; i < this.listeningComplexity; i++)
        {
            inputLayer[i*2] = Neurons.Tanh(listening[i][0])*Neurons.Tanh(outputLayer[i*2]);
            inputLayer[i*2 + 1] = Neurons.Tanh(listening[i][1])*Neurons.Tanh(outputLayer[i*2 + 1]);
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new Ears(ball, this.listeningComplexity, this.maxRadius);
    }
}