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
        c = 0.5f + Mathf.Sin(c*Mathf.Exp(C*8))*0.5f;
        x = Neurons.Tanh(x);
        m = Neurons.Sigmoid(m);
        
        Neuron to = this.ball.neurons.updatableNeurons.Sweep(a);
        Neuron from = this.ball.neurons.updatableNeurons.Sweep(b);
        Neuron replace = this.ball.neurons.updatableNeurons.Sweep(c);

        if(!to.connections.ContainsKey(from))
        {
            if(replace != from && to.connections.ContainsKey(replace))
            {
                to.connections.Remove(replace);
                to.connections.Add(from, x);
            }
        }
        else
        {
            to.connections[from] = x*m + to.connections[from]*(1 - m);
        }
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new SelfSurgeon(ball);
    }
}