using System;
using System.Collections.Generic;
using UnityEngine;
public class Neuron
{
    public readonly Dictionary<Neuron, float> connections;
    public readonly Func<float, float> bounding;
    private float _value;

    public float value
    {
        get
        {
            if(Single.IsNaN(this._value))
            {
                if(this.IsUpdateable())
                {
                    this.Update();
                    if(!Single.IsNaN(this._value))
                    {
                        return this._value;
                    }
                }
                this._value = Utils.GetRandomWeigthLin(BallPit.rand);
            }
            return this._value;
        }
        set
        {
            if(!Single.IsNaN(value))
            {
                this._value = this.bounding != null ? this.bounding(value) : value;
            }
        }
    }

    public Neuron(Neuron[] neurons, Func<float, float> bounding, float value = 0) : this(bounding, value)
    {
        for(int i = 0, l = neurons.Length; i < l; i++)
        {
            this.connections[neurons[i]] = Utils.GetRandomWeigthLin(BallPit.rand);
        }
    }
    public Neuron(Func<float, float> bounding, float value = 0) : this(new Dictionary<Neuron, float>(), bounding, value) {}
    public Neuron(Dictionary<Neuron, float> connections, Func<float, float> bounding, float value = 0)
    {
        this.connections = connections;
        this.bounding = bounding;
        this.value = value;
    }

    public bool IsUpdateable()
    {
        return this.connections.Count != 0;
    }

    public void Update()
    {
        int count = this.connections.Count;
        if(count > 0)
        {
            float y = 0;
            try
            {
                Neuron[] keys = new Neuron[count];
                this.connections.Keys.CopyTo(keys, 0);
                foreach(Neuron neuron in keys)
                {
                    float w = 0;
                    if(!this.connections.TryGetValue(neuron, out w) || Single.IsNaN(w))
                    {
                        this.connections.Remove(neuron);
                        this.connections.Add(neuron, Utils.GetRandomWeigthLin(BallPit.rand));
                    }
                    y += w*neuron.value;
                }
                this.value = y;
            }
            catch
            {
                return; //Failed to update
            }
        }
    }
}