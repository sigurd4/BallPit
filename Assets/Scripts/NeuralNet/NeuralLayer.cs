using System;
public abstract class NeuralLayer : PopulatedLayer
{
    private Neuron[] neurons;
    private readonly PopulatedLayer prevLayer;

    public NeuralLayer(PopulatedLayer prevLayer, int size) : base(size)
    {
        this.prevLayer = prevLayer;
    }
    
    private void GenerateNeurons()
    {
        if(this.neurons == null)
        {
            this.neurons = new Neuron[this.size];
            for(int i = 0; i < this.size; i++)
            {
                this.neurons[i] = new Neuron(this.prevLayer);
            }
        }
    }

    public Neuron GetNeuron(int i)
    {
        this.GenerateNeurons();
        return this.neurons[(i + this.size) % this.size];
    }

    private float GetNeuronValue(Neuron neuron)
    {
        return this.Bounding(neuron.GetValue());
    } 

    public override float[] GetValues()
    {
        this.GenerateNeurons();
        return Array.ConvertAll(this.neurons, this.GetNeuronValue);
    }

    public new class Sigmoid : NeuralLayer
    {
        public Sigmoid(PopulatedLayer prevLayer, int size) : base(prevLayer, size) {}

        public override float Bounding(float x)
        {
            return Neurons.Sigmoid(x);
        }
    }
    
    public new class Tanh : NeuralLayer
    {
        public Tanh(PopulatedLayer prevLayer, int size) : base(prevLayer, size) {}

        public override float Bounding(float x)
        {
            return Neurons.Tanh(x);
        }
    }
    public new class Lin : NeuralLayer
    {
        public Lin(PopulatedLayer prevLayer, int size) : base(prevLayer, size) {}

        public override float Bounding(float x)
        {
            return Neurons.Lin(x);
        }
    }
    public new class ReLU : NeuralLayer
    {
        private readonly float bend;

        public ReLU(PopulatedLayer prevLayer, int size, float bend = 0) : base(prevLayer, size)
        {
            this.bend = bend;
        }

        public override float Bounding(float x)
        {
            return Neurons.ReLU(x, this.bend);
        }
    }
}