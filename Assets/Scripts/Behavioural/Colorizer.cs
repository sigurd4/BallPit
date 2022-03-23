using UnityEngine;
public class Colorizer : Behavioural
{
    public Colorizer(Ball ball) : base(ball, 0, 3)
    {

    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = this.ball.fatigueCoefficient;
        //CONTROL COLOR:
        Color color = new Color(
            fatigueCoefficient*(0.5f + Neurons.Tanh(outputLayer[0])*0.5f),
            fatigueCoefficient*(0.5f + Neurons.Tanh(outputLayer[1])*0.5f),
            fatigueCoefficient*(0.5f + Neurons.Tanh(outputLayer[2])*0.5f)
        );
        this.ball.meshrenderer.material.color = color;
    }
}