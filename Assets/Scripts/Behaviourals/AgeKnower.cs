using UnityEngine;
public class AgeKnower : Behavioural
{
    public AgeKnower(Ball ball) : base(ball, 1, 1)
    {

    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = 1 - Mathf.Exp(-Mathf.Pow(this.ball.age, Neurons.Sigmoid(outputLayer[0])));
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new AgeKnower(ball);
    }
}