using UnityEngine;
public class AgeKnower : Behavioural
{
    private readonly float scale;
    public AgeKnower(Ball ball, float scale) : base(ball, 1, 0)
    {
        this.scale = scale;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = 1 - Mathf.Exp(-this.ball.age*this.scale);
    }
}