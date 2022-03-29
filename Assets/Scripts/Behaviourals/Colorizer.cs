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
        Vector3 c = fatigueCoefficient == 0 ? Vector3.zero : fatigueCoefficient*new Vector3(
            Neurons.Sigmoid(outputLayer[0]),
            Neurons.Sigmoid(outputLayer[1]),
            Neurons.Sigmoid(outputLayer[2])
        );
        this.ball.meshrenderer.enabled = true;

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
    
    public override Behavioural Clone(Ball ball)
    {
        ball.meshrenderer.material.color = this.ball.meshrenderer.material.color;
        return new Colorizer(ball, this.fatigueCurve, this.responsiveness);
    }
}