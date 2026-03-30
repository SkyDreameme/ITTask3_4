
namespace ConveyorSimulation.Core.Interfaces;

public interface IMechanic
{
    string Name { get; }
    void Repair(Conveyor conveyor);
}
