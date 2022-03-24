using System;
using UnityEngine;
public abstract class Layer
{
    public readonly int size;
    
    public Layer(int size)
    {
        if(size <= 0) throw new ArgumentOutOfRangeException("Invalid layer size: " + size);
        this.size = size;
    }

    public abstract float Bounding(float x);

    public class Sigmoid : NonPopulatedLayer
    {
        public Sigmoid(int size) : base(size) {}

        public override float Bounding(float x)
        {
            return Neurons.Sigmoid(x);
        }

        public override PopulatedLayer ConnectTo(PopulatedLayer layer)
        {
            return new NeuralLayer.Sigmoid(layer, this.size);
        }
    }
    
    public class Tanh : NonPopulatedLayer
    {
        public Tanh(int size) : base(size) {}

        public override float Bounding(float x)
        {
            return Neurons.Tanh(x);
        }

        public override PopulatedLayer ConnectTo(PopulatedLayer layer)
        {
            return new NeuralLayer.Tanh(layer, this.size);
        }
    }
    public class Lin : NonPopulatedLayer
    {
        public Lin(int size) : base(size) {}

        public override float Bounding(float x)
        {
            return Neurons.Lin(x);
        }

        public override PopulatedLayer ConnectTo(PopulatedLayer layer)
        {
            return new NeuralLayer.Lin(layer, this.size);
        }
    }
    public class ReLU : NonPopulatedLayer
    {
        private readonly float bend;

        public ReLU(int size, float bend = 0) : base(size)
        {
            this.bend = bend;
        }

        public override float Bounding(float x)
        {
            return Neurons.ReLU(x, this.bend);
        }

        public override PopulatedLayer ConnectTo(PopulatedLayer layer)
        {
            return new NeuralLayer.ReLU(layer, this.size, this.bend);
        }
    }
    public class Input : PopulatedLayer
    {
        private static System.Random inputGen = new System.Random();
        public readonly float[] input;

        public Input(int size) : this(new float[size]) {}
        public Input(float[] input) : base(input.Length)
        {
            this.input = input;

            byte[] bytes = new byte[32*this.size];
            Layer.Input.inputGen.NextBytes(bytes);
            for(int i = 0; i < this.size; i++)
            {
                this.input[i] = Neurons.Modulus(BitConverter.ToSingle(bytes, 32*i));
            }
        }
        
        public override float Bounding(float x)
        {
            Neurons.Finite(x);
            return Mathf.Min(1, Mathf.Max(-1, x));
        }

        public override float[] GetValues()
        {
            return Array.ConvertAll(this.input, this.Bounding);
        }
    }
}