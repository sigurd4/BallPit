using UnityEngine;
public class NoiseGenerator : Behavioural
{
    public NoiseGenerator(Ball ball, int count) : base(ball, count, count)
    {

    }
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        for(int i = 0; i < this.inputLayerNodes; i++)
        {
            float x = Utils.GetRandomWeigthLin(BallPit.rand);
            inputLayer[i] = Mathf.Sign(x)*Mathf.Pow(Mathf.Abs(x), Neurons.Sigmoid(outputLayer[i]));
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new NoiseGenerator(ball, this.inputLayerNodes);
    }
}