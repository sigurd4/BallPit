public class FatigueKnower : Behavioural
{
    private readonly float[] memory;
    private readonly int memoryLength;
    public FatigueKnower(Ball ball, int memory) : base(ball, 1 + memory*2, 0)
    {
        this.memory = new float[memory];
        this.memoryLength = memory;
    }
    
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        inputLayer[0] = this.ball.fatigueCoefficient;
        for(int i = 0; i < this.memoryLength; i++)
        {
            inputLayer[1 + i] = this.memory[i];
            inputLayer[1 + this.memoryLength + i] = this.memory[i] - inputLayer[i];
        }
    }
}