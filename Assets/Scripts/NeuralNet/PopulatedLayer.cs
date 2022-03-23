public abstract class PopulatedLayer : Layer
{
    public PopulatedLayer(int size) : base(size) {}
    public abstract float[] GetValues();
    
    public static LayerGroup<PopulatedLayer> operator +(PopulatedLayer a, LayerGroup<NonPopulatedLayer> b)
    {
        int lc = b.LayerCount();
        if(lc == 0)
        {
            return new LayerGroup<PopulatedLayer>(a);
        }
        PopulatedLayer[] array = new PopulatedLayer[lc + 1];
        array[0] = a;
        for(int i = 0; i < lc; i++)
        {
            array[i + 1] = b.GetLayer(i).ConnectTo(array[i]);
        }
        return new LayerGroup<PopulatedLayer>(array);
    }
}