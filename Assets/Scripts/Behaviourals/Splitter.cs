using UnityEngine;
using System;
public class Splitter : Behavioural
{
    private long timeSinceSplit;
    private float cooldown;

    private float force;
    public Splitter(Ball ball, float cooldown, float force) : base(ball, 1, 5)
    {
        this.cooldown = cooldown;
        this.timeSinceSplit = 0;
        this.force = force;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        bool canPlace = false;

        float p = Neurons.Sigmoid(outputLayer[0]);
        
        float d = Mathf.Exp(-this.timeSinceSplit*this.cooldown);
        if(p > d)
        {
            p = p - d;
            float h = 1 - Neurons.Sigmoid(outputLayer[1]);
            if(p > h)
            {
                float mass = this.ball.mass*0.99f;
                
                float mass1 = mass*(1 - p);
                float mass2 = mass*p;

                if(mass2 > Ball.minVolume*this.ball.density && mass1 > Ball.minVolume*this.ball.density)
                {
                    float cradius = Utils.Cbrt(mass1/this.ball.density/Utils.spherePerCubeVolume);
                    
                    Vector3 fnorm = this.ball.transform.forward.normalized;

                    float dist = (Neurons.Tanh(outputLayer[2]) + 1f)*1f;

                    Vector3 fdir = fnorm*(this.ball.radius + cradius*2 + Single.Epsilon);

                    this.ball.gameObject.SetActive(false);
                    canPlace = !Physics.Raycast(this.ball.position, fdir, fdir.magnitude) && !Physics.Raycast(this.ball.position + fdir, -fdir, fdir.magnitude);
                    this.ball.gameObject.SetActive(true);

                    if(canPlace)
                    {
                        Vector3 pos1 = fnorm*(this.ball.radius + cradius + Single.Epsilon)*mass2/mass;
                        Vector3 pos2 = -fnorm*(this.ball.radius + cradius + Single.Epsilon)*mass1/mass;
                        this.timeSinceSplit = 0;
                        Ball child = this.ball.Duplicate();
                        if(child != null)
                        {
                            float forceMagnitude = this.force*this.ball.fatigueCoefficient*(0.001f + (1 - 0.001f)*Neurons.Sigmoid(Mathf.Pow(Neurons.Sigmoid(outputLayer[3]), Neurons.Tanh(outputLayer[4]))));

                            child.parent = this.ball;

                            child.density = this.ball.density;
                            child.velocity = this.ball.velocity;

                            if(!Single.IsNaN(mass1) && !Single.IsNaN(mass2) && !Single.IsInfinity(mass1) && !Single.IsInfinity(mass2))
                            {
                                this.ball.mass = mass1;
                                child.mass = mass2;

                                this.ball.position = this.ball.position + pos1;
                                child.position = this.ball.position + pos2;
                                
                                Vector3 force = (pos2 - pos1).normalized*forceMagnitude;
                                this.ball.rigidbody.AddForce(-force);
                                child.rigidbody.AddForce(force);
                                this.ball.AddFatigue(forceMagnitude*forceMagnitude/mass*Time.timeScale);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            this.timeSinceSplit++;
        }
        inputLayer[0] = canPlace ? 1 : 0;
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new Splitter(ball, this.cooldown, this.force);
    }
}