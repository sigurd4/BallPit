using System;
public class Behavioural
{
    public readonly int inputLayerNodes;
    public readonly int outputLayerNodes;
    protected readonly Ball ball;

    public Behavioural(Ball ball, int inputLayerNodes, int outputLayerNodes)
    {
        this.ball = ball;
        this.inputLayerNodes = inputLayerNodes;
        this.outputLayerNodes = outputLayerNodes;
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {
        
    }

    public void UpdateNeurons(Neurons neurons, int startIn, int startOut)
    {
        Neuron[] input = neurons.Get(startIn, this.inputLayerNodes);
        Neuron[] output = neurons.Get(startOut, this.outputLayerNodes);

        float[] inVal = Array.ConvertAll(input, (Neuron n) => n.value);
        float[] outVal = Array.ConvertAll(output, (Neuron n) => n.value);

        this.UpdateNeurons(inVal, outVal);

        for(int i = 0; i < this.inputLayerNodes; i++)
        {
            input[i].value = inVal[i];
        }
    }
    protected virtual void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {

    }
}