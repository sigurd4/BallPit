using System;
using UnityEngine;
using System.Collections.Generic;
public class Layer
{
    public readonly int size;
    public readonly Func<float, float> bounding;
    
    public Layer(int size) : this(size, Neurons.Lin) {}
    public Layer(int size, Func<float, float> bounding)
    {
        if(size <= 0) throw new ArgumentOutOfRangeException("Invalid layer size: " + size);
        this.size = size;
        this.bounding = bounding;
    }

    public Neuron[] GenerateConnectedNeurons(Neuron[] neurons)
    {
        List<Neuron> result = new List<Neuron>();
        for(int i = 0; i < size; i++)
        {
            result.Add(new Neuron(neurons, this.bounding, 0));
        }
        return result.ToArray();
    }

    public static LayerGroup operator *(Layer layer, int m)
    {
        return ((LayerGroup)layer)*m;
    }
}