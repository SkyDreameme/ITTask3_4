namespace ConveyorSimulation.Core.Models;

public class Part
{
    public int Id { get; init; }
    public string Type { get; init; } = "Standard";
    public double Weight { get; init; }

    public Part(int id, string type, double weight)
    {
        Id = id;
        Type = type;
        Weight = weight;
    }
}
