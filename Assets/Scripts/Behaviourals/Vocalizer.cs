using UnityEngine;
using System;
public class Vocalizer : Behavioural
{
    private readonly int voiceComplexity;
    private readonly float vocalStrength;
    public Vocalizer(Ball ball, int voiceComplexity, float vocalStrength) : base(ball, 0, voiceComplexity + 1)
    {
        this.voiceComplexity = voiceComplexity;
        this.vocalStrength = vocalStrength;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        this.ball.voice = new float[this.voiceComplexity];

        float volume = ball.volume;

        float m = Neurons.Sigmoid(outputLayer[0])*this.vocalStrength;
        float P = 0;
        for(int i = 0; i < this.voiceComplexity; i++)
        {
            float a = Neurons.Tanh(outputLayer[i + 1])*m*this.ball.fatigueCoefficient;
            
            this.ball.voice[i] = a - this.ball.voice[i];
            if(Single.IsNaN(this.ball.voice[i]))
            {
                this.ball.voice[i] = 0;
                continue;
            }
        }
        this.ball.AddFatigue(volume*Mathf.Pow(P, 2)/2/Utils.atmosphericPressure/Utils.speedOfSoundSquared);
    }
}