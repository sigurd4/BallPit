using UnityEngine;
public class MassKnower : Behavioural
{
    private readonly float scale;
    public MassKnower(Ball ball, float scale) : base(ball, 1, 0)
    {
        this.scale = scale;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = Neurons.Tanh(this.ball.GetComponent<Rigidbody>().mass*scale);
    }
}