using UnityEngine;
public class SelfSurgeon : Behavioural
{
    public SelfSurgeon(Ball ball) : base(ball, 0, 6)
    {
        
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        this.ModifyWeights(outputLayer[0], outputLayer[1], outputLayer[2], outputLayer[3], outputLayer[4], outputLayer[5]);
    }
    
    protected virtual void ModifyWeights(float a, float A, float b, float B, float x, float m)
    {
        a = 0.5f + Mathf.Sin(a*Mathf.Exp(A*6))*0.5f;
        b = 0.5f + Mathf.Sin(b*Mathf.Exp(B*8))*0.5f;
        x = Neurons.Tanh(x);
        m = Neurons.Sigmoid(m);
        
        Neuron to = this.ball.neurons.updatableNeurons.Sweep(a);
        Neuron from = this.ball.neurons.updatableNeurons.Sweep(b);

        if(!to.connections.ContainsKey(from))
        {
            to.connections.Add(from, 0);
        }
        to.connections[from] = x*m + to.connections[from]*(1 - m);
    }
}