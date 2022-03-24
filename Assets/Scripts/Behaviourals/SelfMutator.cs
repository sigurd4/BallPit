using System;
public class SelfMutator : SelfSurgeon
{
    private readonly int degree;
    public SelfMutator(Ball ball, int degree) : base(ball)
    {
        this.degree = degree;
    }

    protected override void ModifyWeights(float a, float A, float b, float B, float c, float C, float x, float m)
    {
        x = Utils.GetRandomWeigthLin(new Random((int)(x*Int32.MaxValue)));
        base.ModifyWeights(a, A, b, B, c, C, x, m);
    }
}