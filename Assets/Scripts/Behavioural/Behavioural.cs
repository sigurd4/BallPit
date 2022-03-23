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

    public void UpdateNeurons(float[] inputLayer, int startIn, float[] outputLayer, int startOut)
    {
        float[] subIn = new float[this.inputLayerNodes];
        float[] subOut = new float[this.outputLayerNodes];
        
        if(startIn + this.inputLayerNodes > inputLayer.Length)
        {
            throw new ArgumentOutOfRangeException("Cannot reach item of index " + (startIn + this.inputLayerNodes) + " in array of length: " + inputLayer.Length);
        }
        if(startOut + this.outputLayerNodes > outputLayer.Length)
        {
            throw new ArgumentOutOfRangeException("Cannot reach item of index " + (startOut + this.outputLayerNodes) + " in array of length: " + outputLayer.Length);
        }
        Array.Copy(inputLayer, startIn, subIn, 0, this.inputLayerNodes);
        Array.Copy(outputLayer, startOut, subOut, 0, this.outputLayerNodes);

        this.UpdateNeurons(subIn, subOut);
        
        Array.Copy(subIn, 0, inputLayer, startIn, this.inputLayerNodes);
    }
    protected virtual void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {

    }
}