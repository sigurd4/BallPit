using UnityEngine;
using System.Collections.Generic;
public class ChargeManipulator : Behavioural
{
    public float scale;
    public ChargeManipulator(Ball ball, float scale) : base(ball, 0, 1)
    {        
        this.scale = scale;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float mass = this.ball.rigidbody.mass;

        float charge = Neurons.Tanh(outputLayer[0])*this.scale*fatigueCoefficient*mass;
        
        float radius = this.ball.GetRadius();
        float energy = Utils.k*(Mathf.Pow(charge, 3) - Mathf.Pow(this.ball.charge, 3))/radius;
        this.ball.charge = charge;

        this.ball.Fatigue(energy);
        
        if(this.ball.charge != 0)
        {
            HashSet<Ball> balls = Ball.GetLiving();
            Vector3 force = Vector3.zero;

            foreach(Ball other in balls)
            {
                if(other == null || other == this.ball) continue;
                Vector3 r = other.transform.position - this.ball.transform.position;

                force += r.normalized*other.charge/Mathf.Pow(r.magnitude, 2);
            }
            force *= this.ball.charge*Utils.k;
            this.ball.rigidbody.AddForce(force);
        }
    }
}