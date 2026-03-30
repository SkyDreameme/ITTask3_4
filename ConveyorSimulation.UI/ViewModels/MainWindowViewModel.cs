using System;
using System.Collections.ObjectModel;
using ConveyorSimulation.Core.Services;

namespace ConveyorSimulation.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        public ObservableCollection<ConveyorViewModel> Conveyors { get; } = new();
        private int _conveyorCounter = 1;
        private bool _disposed = false;

        public Command AddConveyorCommand { get; }
        public Command StartAllCommand { get; }
        public Command StopAllCommand { get; }
        public Command ClearAllCommand { get; }

        public MainWindowViewModel()
        {
            AddConveyorCommand = new Command(AddConveyor);
            StartAllCommand = new Command(StartAll);
            StopAllCommand = new Command(StopAll);
            ClearAllCommand = new Command(ClearAll);
        }

        private void AddConveyor()
        {
            if (Conveyors.Count >= 10)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Максимум конвейеров!");
                return;
            }

            var config = new ConveyorConfig
            {
                Name = $"Line-{_conveyorCounter++}",
                MaxMaterialCapacity = 100,
                ProductionSpeedMs = new Random().Next(500, 1000),
                BreakProbability = 0.02
            };

            var vm = new ConveyorViewModel(config);
            Conveyors.Add(vm);
            vm.Start();
        }

        private void StartAll()
        {
            foreach (var c in Conveyors)
            {
                try { c.Start(); }
                catch { }
            }
        }

        private void StopAll()
        {
            foreach (var c in Conveyors)
            {
                try { c.Stop(); }
                catch { }
            }
        }

        private void ClearAll()
        {
            foreach (var c in Conveyors)
            {
                c.Dispose();
            }
            Conveyors.Clear();
            _conveyorCounter = 1;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            ClearAll();
        }

        ~MainWindowViewModel() => Dispose();
    }

    public class Command : System.Windows.Input.ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public Command(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}