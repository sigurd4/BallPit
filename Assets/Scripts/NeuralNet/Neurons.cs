using System;
using UnityEngine;
using System.Collections.Generic;

public class Neurons
{
    private readonly Neuron[] neurons;
    public readonly int count;

    public Neurons(Neuron[] neurons)
    {
        this.neurons = neurons;
        this.count = neurons.Length;
    }

    public Neuron this[int i]
    {
        get
        {
            if(i < 0) return this[this.count + i];
            return this.neurons[i];
        }
    }

    private Neuron[] GetUpdateableNeurons()
    {
        List<Neuron> neurons = new List<Neuron>();
        for(int i = 0; i < this.count; i++)
        {
            Neuron n = this[i];
            if(n.IsUpdateable())
            {
                neurons.Add(n);
            }
        }
        return neurons.ToArray();
    }

    public void UpdateNeurons(int ncount)
    {
        Neuron[] neurons = this.GetUpdateableNeurons();
        int l = neurons.Length;
        int[] hits = new int[ncount];
        for(int n = 0; n < ncount; n++)
        {
            int i = Utils.NextFiltered(BallPit.rand, 0, l, hits, n);
            hits[n] = i;
            neurons[i].Update();
        }
    }

    public Neuron Sweep(float x)
    {
        int i = (int)Mathf.Max(Mathf.Min(Mathf.Floor(x*this.count), this.count - 1), 0);
        return this[i];
    }

    public Neuron[] Get(int start, int count)
    {
        Neuron[] neurons = new Neuron[count];
        for(int i = 0; i < count; i++)
        {
            neurons[i] = this[i + start];
        }
        return neurons;
    }
    
    #region bounding-functions
        public static float Finite(float x)
        {
            if(Single.IsNaN(x)) return 0;
            //return Mathf.Min(1, Mathf.Max(-1, x));
            if(Single.IsPositiveInfinity(x)) return 1;
            if(Single.IsNegativeInfinity(x)) return -1;
            return x;
        }
        public static float Modulus(float x)
        {
            return -(-(x%1))%1;
        }
        public static float Sigmoid(float x)
        {
            x = Neurons.Finite(x);
            return 1f/(1f + Mathf.Exp(-x));
        }
        
        public static float Tanh(float x)
        {
            x = Neurons.Finite(x);
            float a = Mathf.Exp(x);
            float b = Mathf.Exp(-x);
            return (a - b)/(a + b);
        }

        public static float Lin(float x)
        {
            x = Neurons.Finite(x);
            //return Mathf.Min(1, Mathf.Max(-1, x));
            return x;
        }
        
        public static float ReLU(float x)
        {
            //x = Neurons.Finite(x);
            //return Mathf.Min(1, Mathf.Max(0, x));
            //return Mathf.Max(0, x);
            return Neurons.ReLU(x, 0.1f);
        }
        public static float ReLU(float x, float bend)
        {
            x = Neurons.Finite(x);
            //return Mathf.Min(1, Mathf.Max(-1, Mathf.Max(0, x)*(1 - bend) + x*bend));
            return Mathf.Max(0, x)*(1f - bend) + x*bend;
        }
    #endregion

    public static LayerGroup GenerateHiddenLayers(int nodeCount)
    {
        List<Layer> g = new List<Layer>();
        for(int n = 0; n < nodeCount; n++)
        {
            int size = BallPit.rand.Next((nodeCount - n)/2, nodeCount - n);
            if(size > 0)
            {
                n += size;
                switch(BallPit.rand.Next(0, 5))
                {
                    case 0: g.Add(new Layer(size, Neurons.Lin)); break;
                    case 4: g.Add(new Layer(size, Neurons.ReLU)); break;
                    case 2: g.Add(new Layer(size, Neurons.Sigmoid)); break;
                    case 3: g.Add(new Layer(size, Neurons.Tanh)); break;
                }
            }
        }
        g.Sort((Layer a, Layer b) => {
            return BallPit.rand.Next(0, 2)*2 - 1;
        });
        return g.ToArray();
    }
}