using UnityEngine;
public class Colorizer : Behavioural
{
    private readonly float fatigueCurve;
    private readonly float responsiveness;
    private Vector3 color;
    public Colorizer(Ball ball, float fatigueCurve, float responsiveness) : base(ball, 0, 3)
    {
        this.fatigueCurve = fatigueCurve;
        this.responsiveness = responsiveness;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        float fatigueCoefficient = Mathf.Pow(this.ball.fatigueCoefficient, this.fatigueCurve);
        //CONTROL COLOR:
        Vector3 c = new Vector3(
            fatigueCoefficient*(0.5f + Neurons.Tanh(outputLayer[0])*0.5f),
            fatigueCoefficient*(0.5f + Neurons.Tanh(outputLayer[1])*0.5f),
            fatigueCoefficient*(0.5f + Neurons.Tanh(outputLayer[2])*0.5f)
        );

        if(this.color == null)
        {
            this.color = c;
        }
        else
        {
            float x = Mathf.Exp(-responsiveness*Time.deltaTime);
            this.color = (c*(1-x) + this.color*x);
        }

        Color color = new Color(c[0], c[1], c[2]);
        this.ball.meshrenderer.material.color = color;
    }
}