namespace ConveyorSimulation.Core.Services;

public class ConveyorConfig
{
    public string Name { get; set; } = "Default";
    public int MaxMaterialCapacity { get; set; } = 100;
    public int ProductionSpeedMs { get; set; } = 500;
    public double BreakProbability { get; set; } = 0.05;
    public int RepairTimeMs { get; set; } = 3000;
}
