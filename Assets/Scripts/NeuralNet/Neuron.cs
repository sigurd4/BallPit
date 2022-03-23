using System;
using System.Text;
using UnityEngine;
public class Neuron
{
    public readonly PopulatedLayer prevLayer;
    public readonly float[] coefficients;

    public Neuron(PopulatedLayer prevLayer)
    {
        this.prevLayer = prevLayer;
        this.coefficients = new float[this.prevLayer.size];

        byte[] bytes = new byte[32*this.prevLayer.size];
        //this.weigthGen.NextBytes(bytes);
        for(int i = 0; i < this.prevLayer.size; i++)
        {
            this.coefficients[i] = Utils.GetRandomWeigthLin(BallPit.rand);
            //this.coefficients[i] = Utils.GetRandomSign(this.weigthGen)*Neurons.Modulus(BitConverter.ToSingle(bytes, 32*i));
        }
    }
    public Neuron(float[] coefficients)
    {
        this.coefficients = Array.ConvertAll(coefficients, this.BoundCoeff);
    }

    private float BoundCoeff(float coefficient)
    {
        return Mathf.Pow(Neurons.Tanh(coefficient), 10);
    }

    public float GetValue()
    {
        float[] array = this.prevLayer.GetValues();
        float y = 0;
        float ctot = 0;
        for(int i = 0; i < this.prevLayer.size; i++)
        {
            float c = this.coefficients[i];
            y += c*array[i];
            ctot += c;
        }
        if(y != 0)
        {
            //y /= ctot;
        }
        return y;
    }
}