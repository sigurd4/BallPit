using System.Linq;
using System;
using System.Collections.Generic;

public class LayerGroup
{
    public readonly int count;
    private readonly Layer[] layers;

    public LayerGroup() : this(new Layer[0]) {}
    public LayerGroup(Layer layer) : this(new Layer[]{layer}) {}
    public LayerGroup(Layer[] layers)
    {
        this.layers = layers;
        this.count = layers.Length;
    }
    public Layer this[int i]
    {
        get
        {
            if(i < 0)
            {
                return this[this.count + i];
            }
            return this.layers[i];
        }
        set
        {
            if(i < 0)
            {
                this[this.count + i] = value;
            }
            this.layers[i] = value;
        }
    }
    public Layer[] GetLayers()
    {
        return this.layers;
    }

    public Neurons GenerateNeurons()
    {
        List<Neuron> result = new List<Neuron>();
        Neuron[] n = new Neuron[0];
        for(int i = 0; i < this.count; i++)
        {
            n = this[i].GenerateConnectedNeurons(n);
            result.AddRange(n);
        }
        return new Neurons(result.ToArray());
    }
    
    public static implicit operator LayerGroup(Layer layer)
    {
        return new LayerGroup(layer);
    }
    public static implicit operator LayerGroup(Layer[] array)
    {
        return new LayerGroup(array);
    }

    public static implicit operator Layer[](LayerGroup group)
    {
        return group.GetLayers();
    }
    public static LayerGroup operator +(LayerGroup a, LayerGroup b)
    {
        return new LayerGroup(a.GetLayers().Concat(b.GetLayers()).ToArray());
    }
    public static LayerGroup operator *(LayerGroup group, int repeats)
    {
        LayerGroup g = new Layer[group.count*repeats];
        for(int i = 0; i < group.count; i++)
        {
            Layer layer = group.layers[i];
            for(int j = 0; j < repeats; j++)
            {
                g.layers[i + j*group.count] = layer;
                layer = new Layer(layer.size, layer.bounding);
            }
        }
        return g;
    }
}