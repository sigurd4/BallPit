using System.Linq;
using System;
using System.Collections.Generic;

public class LayerGroup<T> where T : Layer
{
    private readonly int count;
    private readonly T[] layers;
    public LayerGroup(T layers, int count = 1) : this(new T[]{layers}, count) {}
    public LayerGroup(T[] layers, int count = 1)
    {
        this.layers = layers;
        this.count = count;
    }

    public int LayerCount()
    {
        return this.count*layers.Length;
    }
    public T GetLayer(int i)
    {
        int size = this.LayerCount();
        if(i < 0)
        {
            i = size + i;
        }
        return this.layers[i/* % size*/];
    }
    public G[] GetLayers<G>() where G : T
    {
        List<G> g = new List<G>();
        T[] layers = this.GetLayers();
        for(int i = 0, l = layers.Length; i < l; i++)
        {
            if(layers[i] is G)
            {
                g.Add((G)layers[i]);
            }
        }
        return g.ToArray();
    }
    public T[] GetLayers()
    {
        if(this.count < 0) throw new ArgumentOutOfRangeException("Invalid layer count in layergroup: " + this.count);
        List<T> a = new List<T>(this.layers);
        for(int i = 1; i < this.count; i++)
        {
            a.AddRange(this.layers);
        }
        return a.ToArray();
    }
    
    public static implicit operator LayerGroup<T>(T[] array)
    {
        return new LayerGroup<T>(array);
    }

    public static implicit operator T[](LayerGroup<T> group)
    {
        return group.GetLayers();
    }
    public static LayerGroup<T> operator +(LayerGroup<T> a, LayerGroup<T> b)
    {
        return new LayerGroup<T>((a.GetLayers().Concat(b.GetLayers())).ToArray<T>());
    }
    public static LayerGroup<T> operator *(LayerGroup<T> group, int repeats)
    {
        return new LayerGroup<T>(group.GetLayers(), group.count*repeats);
    }
}