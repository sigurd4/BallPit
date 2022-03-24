using System;
using UnityEngine;

public class Neurons
{
    public readonly LayerGroup<PopulatedLayer> structure;

    public Neurons(Layer.Input inputLayer, LayerGroup<NonPopulatedLayer> otherLayers) : this(inputLayer + otherLayers) {}
    public Neurons(LayerGroup<PopulatedLayer> structure)
    {
        this.structure = structure;
    }

    public float[] GetOutputLayerValues()
    {
        return this.structure.GetLayer(-1).GetValues();
    }
    
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
        x = Neurons.Finite(x);
        //return Mathf.Min(1, Mathf.Max(0, x));
        return Mathf.Max(0, x);
    }
    public static float ReLU(float x, float bend)
    {
        x = Neurons.Finite(x);
        //return Mathf.Min(1, Mathf.Max(-1, Mathf.Max(0, x)*(1 - bend) + x*bend));
        return Mathf.Max(0, x)*(1f - bend) + x*bend;
    }
}