using UnityEngine;
public class FreeEnergy : Behavioural
{
    public float regen;
    public FreeEnergy(Ball ball, float regen) : base(ball, 0, 0)
    {
        this.regen = regen;
    }
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        this.ball.fatigue *= Mathf.Exp(-this.regen*Time.deltaTime);
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new FreeEnergy(ball, this.regen);
    }
}