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
        this.ball.fatigue *= Mathf.Exp(-1000000*Time.deltaTime);
    }
}