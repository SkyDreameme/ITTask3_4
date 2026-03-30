using System;
using System.Threading.Tasks;
using ConveyorSimulation.Core.Interfaces;

namespace ConveyorSimulation.Core;

public class MechanicImplementation : IMechanic
{
    public string Name { get; }

    public MechanicImplementation(string name)
    {
        Name = name;
    }

    public void AttachToConveyor(Conveyor conveyor)
    {
        conveyor.Broken += OnConveyorBroken;
    }

    private async void OnConveyorBroken(object? sender, EventArgs e)
    {
        if (sender is Conveyor conveyor)
        {
            Console.WriteLine($"{Name}: Выезжаю на ремонт {conveyor.Name}...");
            // имитация времени ремонта
            await Task.Delay(3000);
            conveyor.Fix();
        }
    }

    public void Repair(Conveyor conveyor)
    {
        // явный вызов ремонта, если нужно
        conveyor.Fix();
    }
}
