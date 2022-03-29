using System;
using System.Collections.Generic;
using System.Linq;
public class MemoryStack : Behavioural
{
    private static readonly System.Random rand = new System.Random(/*DateTime.Now.Millisecond*454502*this.GetHashCode()*/);
    public readonly int length;
    public readonly int blocksize;
    public readonly int speed;
    private readonly Stack<float>[] stack;
    private Stack<int> pickOrder;
    public MemoryStack(Ball ball, int length, int blocksize, int speed) : base(ball, length*blocksize, blocksize)
    {
        this.length = length;
        this.blocksize = blocksize;

        this.speed = Math.Min(speed, this.blocksize);
        
        this.stack = new Stack<float>[this.blocksize];
        this.GenerateStacks();
        this.GeneratePickOrder();
    }
    
    public override Behavioural Clone(Ball ball)
    {
        return new MemoryStack(ball, this.length, this.blocksize, this.speed);
    }

    private int[] GetRandomIndexes()
    {
        return Utils.ShuffledIndexes(this.blocksize).Concat(Utils.ShuffledIndexes(this.blocksize)).ToArray<int>();
    }

    private void GeneratePickOrder()
    {
        this.pickOrder = new Stack<int>(this.GetRandomIndexes());
    }
    private int GetNextPick()
    {
        if(this.pickOrder.Count == 0)
        {
            this.GeneratePickOrder();
        }
        return this.pickOrder.Pop();
    }

    private void GenerateStacks()
    {

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
            this.stack[n].Push(Utils.GetRandomPositiveScalar(MemoryStack.rand));
        }
    }
    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        int[] hits = new int[this.speed];
        for(int i = 0; i < this.speed; i++)
        {
            int n = this.GetNextPick();
            this.stack[n].Push(outputLayer[n]);
            this.stack[n].Pop();
            Array.Copy(this.stack[n].ToArray(), 0, inputLayer, n*this.length, this.length);
        }
    }
}