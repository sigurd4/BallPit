using System;
using System.Collections.Generic;
using UnityEngine;
public class Neuron
{
    public readonly Dictionary<Neuron, float> connections;
    private readonly Func<float, float> bounding;
    public float value;

    public Neuron(Neuron[] neurons, Func<float, float> bounding, float value = 0) : this(bounding, value)
    {
        for(int i = 0, l = neurons.Length; i < l; i++)
        {
            this.connections[neurons[i]] = Mathf.Pow(Utils.GetRandomWeigthLin(BallPit.rand), 3);
        }
    }
    public Neuron(Func<float, float> bounding, float value = 0) : this(new Dictionary<Neuron, float>(), bounding, value) {}
    public Neuron(Dictionary<Neuron, float> connections, Func<float, float> bounding, float value = 0)
    {
        this.connections = connections;
        this.value = value;
    }

    public bool IsUpdateable()
    {
        return this.connections.Count != 0;
    }

    public void Update()
    {
        if(this.connections.Count > 0)
        {
            float y = 0;
            foreach(Neuron neuron in this.connections.Keys)
            {
                float c = this.connections[neuron];
                y += c*neuron.value;
            }
            this.value = this.bounding != null ? this.bounding(y) : y;
        }
    }
}