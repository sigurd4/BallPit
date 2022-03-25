using UnityEngine;
public class AgeKnower : Behavioural
{
    private readonly float timescale;
    public AgeKnower(Ball ball, float timescale) : base(ball, 1, 0)
    {
        this.timescale = timescale;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = 1 - Mathf.Exp(-this.ball.age*this.timescale);
    }
}