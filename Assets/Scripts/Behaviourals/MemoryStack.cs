using System;
using System.Collections.Generic;
public class MemoryStack : Behavioural
{
    private static readonly System.Random rand = new System.Random(/*DateTime.Now.Millisecond*454502*this.GetHashCode()*/);
    public readonly int length;
    public readonly int blocksize;
    public readonly int speed;
    private readonly Stack<float>[] stack;
    public MemoryStack(Ball ball, int length, int blocksize, int speed) : base(ball, length*blocksize, blocksize)
    {
        this.length = length;
        this.blocksize = blocksize;

        this.speed = Math.Min(speed, this.blocksize);
        this.stack = new Stack<float>[blocksize];
        
        for(int n = 0; n < this.blocksize; n++)
        {
            this.stack[n] = new Stack<float>();
            this.FillStackRandom(n);
        }
    }

    private void FillStackRandom(int n)
    {
        //Fill stack
        for(int i = 0; i < this.length; i++)
        {
            this.stack[n].Push(Utils.GetRandomMultiplier(MemoryStack.rand));
        }
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        int[] hits = new int[this.speed];
        for(int i = 0; i < this.speed; i++)
        {
            int n = Utils.NextFiltered(BallPit.rand, 0, this.blocksize - 1, hits, i);
            hits[i] = n;
            this.stack[n].Push(outputLayer[n]);
        }

        for(int n = 0; n < this.blocksize; n++)
        {
            while(this.stack[n].Count > this.length)
            {
                this.stack[n].Pop();
            }
            
            Array.Copy(this.stack[n].ToArray(), 0, inputLayer, n*this.length, this.length);
        }
    }
}