using UnityEngine;
public class SelfDigester : Behavioural
{
    public static float digestion = 1/100000000;
    public SelfDigester(Ball ball) : base(ball, 0, 2)
    {

    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;

        float mass = this.ball.GetComponent<Rigidbody>().mass;

        float fatigue = this.ball.fatigue;
        
        //fatigue *= Mathf.Exp(-1000000*Time.timeScale);
        
        float fatigueRecovery = fatigue*(Neurons.Sigmoid(outputLayer[0]) - fatigueCoefficient)*Neurons.Sigmoid(outputLayer[1]);
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
            return false;
        }
        ball.mass = mass - dmass;
        return true;
    }
}