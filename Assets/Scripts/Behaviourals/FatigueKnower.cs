using UnityEngine;
public class FatigueKnower : Behavioural
{
    private readonly float[] memory;
    private readonly int memoryLength;
    public FatigueKnower(Ball ball, int memoryLength) : base(ball, 1 + memoryLength*2, 1)
    {
        this.memory = new float[memoryLength];
        this.memoryLength = memoryLength;
    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = Neurons.Tanh(Mathf.Pow(this.ball.fatigueCoefficient, Neurons.Tanh(outputLayer[0])));
        for(int i = 0; i < this.memoryLength; i++)
        {
            inputLayer[1 + i] = this.memory[i];
            inputLayer[1 + this.memoryLength + i] = this.memory[i] - inputLayer[i];
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new FatigueKnower(ball, this.memoryLength);
    }
}