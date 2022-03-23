using UnityEngine;
public class SelfSurgeon : Behavioural
{
    public SelfSurgeon(Ball ball) : base(ball, 0, 8)
    {
        
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        this.ModifyWeights(outputLayer[0], outputLayer[1], outputLayer[2], outputLayer[3], outputLayer[4], outputLayer[5], outputLayer[6], outputLayer[7]);
    }
    
    protected virtual void ModifyWeights(float a, float A, float b, float B, float c, float C, float x, float m)
    {
        a = 0.5f + Mathf.Sin(a*Mathf.Exp(A*6))*0.5f;
        b = 0.5f + Mathf.Sin(b*Mathf.Exp(B*8))*0.5f;
        c = 0.5f + Mathf.Sin(c*Mathf.Exp(C*9))*0.5f;
        x = Neurons.Tanh(x);
        m = Neurons.Sigmoid(m);
        
        NeuralLayer[] neuralLayers = this.ball.neurons.structure.GetLayers<NeuralLayer>();
        int nl = neuralLayers.Length;
        int i = (int)Mathf.Max(Mathf.Min(Mathf.Floor(a*nl), nl - 1), 0);
        NeuralLayer layer = neuralLayers[i];
        int ls = layer.size;
        int j = (int)Mathf.Max(Mathf.Min(Mathf.Floor(b*ls), ls - 1), 0);
        Neuron neuron = layer.GetNeuron(j);
        int cc = neuron.prevLayer.size;
        int k = (int)Mathf.Max(Mathf.Min(Mathf.Floor(c*cc), cc - 1), 0);
        neuron.coefficients[k] = x*m + neuron.coefficients[k]*(1 - m);
    }
}