using System;

namespace ConveyorSimulation.Core
{
    public class Loader
    {
        public string Name { get; }

        public Loader(string name)
        {
            Name = name;
        }

        public void AttachToConveyor(Conveyor conveyor)
        {
            conveyor.MaterialLow += OnMaterialLow;
        }

        private void OnMaterialLow(object? sender, EventArgs e)
        {
            if (sender is Conveyor conveyor)
            {
                Console.WriteLine($"{Name}: Загрузка материалов...");
                conveyor.LoadMaterials(50);
            }
        }
    }
}