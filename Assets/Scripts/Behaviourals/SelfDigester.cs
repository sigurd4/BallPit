using UnityEngine;
public class SelfDigester : Behavioural
{
    private readonly float digestion;
    public SelfDigester(Ball ball, float digestion) : base(ball, 0, 2)
    {
        this.digestion = digestion;
    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float mass = this.ball.GetComponent<Rigidbody>().mass;

        float fatigue = this.ball.fatigue;
        
        float fatigueRecovery = fatigue*((1 - Mathf.Exp(-this.digestion*Time.deltaTime*Neurons.Sigmoid(outputLayer[0]))) - fatigueCoefficient)*(1 - Mathf.Exp(-this.digestion*Time.deltaTime*Neurons.Sigmoid(outputLayer[1])));
        if(this.BurnMass(fatigueRecovery))
        {
            fatigue -= fatigueRecovery;
        }
        this.ball.fatigue = fatigue;
    }
    
    private bool BurnMass(float energy)
    {
        if(energy == 0) return false;

        float mass = ball.mass;
        float dmass = energy/Utils.speedOfLightSquared;
        if(dmass > mass)
        {
            this.ball.Kill();
            return false;
        }
        ball.mass = mass - dmass;
        return true;
    }
}