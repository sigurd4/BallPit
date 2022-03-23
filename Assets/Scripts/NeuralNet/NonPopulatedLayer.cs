using System.Linq;
public abstract class NonPopulatedLayer : Layer
{
    public NonPopulatedLayer(int size) : base(size) {}
    public abstract PopulatedLayer ConnectTo(PopulatedLayer layer);
    
    public static LayerGroup<NonPopulatedLayer> operator *(NonPopulatedLayer layer, int coefficient)
    {
        return new LayerGroup<NonPopulatedLayer>(layer, coefficient);
    }
    public static LayerGroup<NonPopulatedLayer> operator +(NonPopulatedLayer a, NonPopulatedLayer b)
    {
        return new LayerGroup<NonPopulatedLayer>(new NonPopulatedLayer[]{a, b});
    }
    public static implicit operator LayerGroup<NonPopulatedLayer>(NonPopulatedLayer layer)
    {
        return new LayerGroup<NonPopulatedLayer>(new NonPopulatedLayer[]{layer});
    }
}