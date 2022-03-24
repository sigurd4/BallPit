public class MemoryStack : Behavioural
{
    private static readonly System.Random rand = new System.Random(/*DateTime.Now.Millisecond*454502*this.GetHashCode()*/);
    private readonly int length;
    private readonly int blocksize;
    private readonly float[,] stack;
    public MemoryStack(Ball ball, int length, int blocksize) : base(ball, length*blocksize, blocksize)
    {
        this.stack = new float[length, blocksize];
        
        for(int i = 0; i < this.length; i++)
        {
            for(int j = 0; j < this.blocksize; j++)
            {
                this.stack[i, j] = Utils.GetRandomMultiplier(MemoryStack.rand);
            }
        }
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        for(int i = 0; i < this.length; i++)
        {
            for(int j = 0; j < this.blocksize; j++)
            {
                inputLayer[i*this.blocksize + j] = this.stack[i, j];
                this.stack[i, j] = i + 1 == this.length ? outputLayer[j] : this.stack[i + 1, j];
            }
        }
    }
}