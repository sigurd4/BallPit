using UnityEngine;
using System;
public class Wiggler : Behavioural
{
    private static readonly System.Random rand = new System.Random(/*DateTime.Now.Millisecond*324884564*this.GetHashCode()*/);
    private readonly float wiggle;
    private readonly int polySize;
    public Wiggler(Ball ball, float wiggle, int polySize) : base(ball, 0, polySize)
    {
        this.polySize = polySize;
        this.wiggle = wiggle;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float[] polynomial = new float[this.polySize];
        for(int i = 0; i < this.polySize; i++)
        {
            polynomial[i] = Mathf.Pow(Neurons.Sigmoid(outputLayer[i]), 2);
        }

        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float mass = this.ball.mass;

        Vector3 force = this.GetWiggleVector(polynomial)*Time.deltaTime*this.wiggle*mass*fatigueCoefficient;
        if(!Utils.IsFinite(force))
        {
            return;
        }
        this.ball.rigidbody.AddForce(force);

        this.ball.AddFatigue(force.magnitude*force.magnitude/mass*Time.timeScale);
    }

    private Vector3 GetWiggleVector(float[] polynomial)
    {
        float y = 0;
        for(int i = 0, l = polynomial.Length; i < l; i++)
        {
            if(polynomial[i] != 0.0f)
            {
                float x = Utils.GetRandomMultiplier(Wiggler.rand);
                for(int j = 0; j < i; j++)
                {
                    x *= Utils.GetRandomMultiplier(Wiggler.rand);
                }
                y += x*polynomial[i];
            }
        }
        return Utils.GetRandomDirection(Wiggler.rand)*this.ball.transform.up*y;
    }
}