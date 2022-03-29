using UnityEngine;
public class MassKnower : Behavioural
{
    public MassKnower(Ball ball) : base(ball, 2, 2)
    {

    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = Neurons.Tanh(Mathf.Pow(this.ball.mass, Neurons.Tanh(outputLayer[0])));
        inputLayer[1] = Neurons.Tanh(Mathf.Pow(this.ball.density, Neurons.Tanh(outputLayer[1])));
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new MassKnower(ball);
    }
}