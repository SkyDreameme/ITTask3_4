using System;
using System.Collections.ObjectModel;
using ConveyorSimulation.Core;
using ConveyorSimulation.Core.Services;
using Avalonia.Threading;

namespace ConveyorSimulation.UI.ViewModels
{
    public class ConveyorViewModel : ViewModelBase, IDisposable
    {
        private readonly Conveyor _conveyor;
        private readonly Loader _loader;
        private readonly MechanicImplementation _mechanic;
        private readonly System.Threading.Timer _updateTimer;
        private bool _disposed = false;

        public string Name => _conveyor.Name;

        private string _status = "Ожидание";
        public string Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }

        private int _materialPercent;
        public int MaterialPercent
        {
            get => _materialPercent;
            set => SetField(ref _materialPercent, value);
        }

        private int _partsCount;
        public int PartsCount
        {
            get => _partsCount;
            set => SetField(ref _partsCount, value);
        }

        private bool _isBroken;
        public bool IsBroken
        {
            get => _isBroken;
            set => SetField(ref _isBroken, value);
        }

        public ObservableCollection<PartVisual> Parts { get; } = new();
        private const int MaxPartsDisplay = 5;

        public ConveyorViewModel(ConveyorConfig config)
        {
            _conveyor = new Conveyor(config);
            _loader = new Loader($"Loader-{config.Name}");
            _mechanic = new MechanicImplementation($"Mechanic-{config.Name}");

            _loader.AttachToConveyor(_conveyor);
            _mechanic.AttachToConveyor(_conveyor);

            // подписка на события
            _conveyor.StatusChanged += OnStatusChanged;
            _conveyor.PartProduced += OnPartProduced;

            _updateTimer = new System.Threading.Timer(UpdateState, null, 500, 500);
        }

        private void OnStatusChanged(object? sender, string msg)
        {
            if (_disposed) return;
            Dispatcher.UIThread.Post(() => Status = msg);
        }

        private void OnPartProduced(object? sender, Core.Models.Part part)
        {
            if (_disposed) return;

            Dispatcher.UIThread.Post(() =>
            {
                PartsCount = _conveyor.PartsCount;

                if (Parts.Count >= MaxPartsDisplay)
                    Parts.RemoveAt(0);
                Parts.Add(new PartVisual(part.Id));
            });
        }

        private void UpdateState(object? state)
        {
            if (_disposed) return;

            Dispatcher.UIThread.Post(() =>
            {
                MaterialPercent = _conveyor.MaterialPercent;
                IsBroken = _conveyor.IsBroken;
            });
        }

        public void Start() => _conveyor.Start();
        public void Stop() => _conveyor.Stop();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _conveyor.StatusChanged -= OnStatusChanged;
            _conveyor.PartProduced -= OnPartProduced;
            _conveyor.Stop();
            _updateTimer?.Dispose();
            Parts.Clear();

            GC.SuppressFinalize(this);
        }

        ~ConveyorViewModel() => Dispose();
    }

    public class PartVisual
    {
        public int Id { get; }
        public double Offset { get; set; }
        public PartVisual(int id) => Id = id;
    }
}