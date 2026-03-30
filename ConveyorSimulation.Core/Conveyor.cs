using System;
using System.Threading;
using System.Threading.Tasks;
using ConveyorSimulation.Core.Models;
using ConveyorSimulation.Core.Services;

namespace ConveyorSimulation.Core
{
    public class Conveyor
    {
        public event EventHandler<EventArgs>? MaterialLow;
        public event EventHandler<EventArgs>? Broken;
        public event EventHandler<Part>? PartProduced;
        public event EventHandler<string>? StatusChanged;

        private readonly ConveyorConfig _config;
        private CancellationTokenSource? _cts;
        private bool _isWorking;
        private bool _isBroken;
        private int _currentMaterial;
        private int _partsCount;

        private static readonly Random _globalRandom = new();
        private readonly Random _random;

        public string Name => _config.Name;
        public bool IsWorking => _isWorking && !_isBroken;
        public bool IsBroken => _isBroken;
        public int MaterialPercent => _config.MaxMaterialCapacity > 0
            ? (int)((double)_currentMaterial / _config.MaxMaterialCapacity * 100)
            : 0;
        public int PartsCount => _partsCount;

        public Conveyor(ConveyorConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _currentMaterial = _config.MaxMaterialCapacity;

            lock (_globalRandom)
            {
                _random = new Random(_globalRandom.Next());
            }
        }

        public void Start()
        {
            if (_isWorking) return;
            _isWorking = true;
            _isBroken = false;
            _cts = new CancellationTokenSource();

            Task.Run(() => SimulationLoop(_cts.Token));
            RaiseStatusChanged("Конвейер запущен");
        }

        public void Stop()
        {
            _isWorking = false;

            try
            {
                _cts?.Cancel();
            }
            catch (ObjectDisposedException) { }
        }

        public void LoadMaterials(int amount)
        {
            _currentMaterial = Math.Min(_currentMaterial + amount, _config.MaxMaterialCapacity);
            RaiseStatusChanged($"Материалы: {_currentMaterial}/{_config.MaxMaterialCapacity}");
        }

        public void Fix()
        {
            _isBroken = false;
            RaiseStatusChanged("Отремонтирован");
        }

        private async Task SimulationLoop(CancellationToken token)
        {
            while (_isWorking && !token.IsCancellationRequested)
            {
                if (_isBroken)
                {
                    await Task.Delay(500, token);
                    continue;
                }

                if (_currentMaterial <= 0)
                {
                    MaterialLow?.Invoke(this, EventArgs.Empty);
                    await Task.Delay(1000, token);
                    continue;
                }

                double breakChance;
                lock (_random)
                {
                    breakChance = _random.NextDouble();
                }

                if (breakChance < _config.BreakProbability)
                {
                    _isBroken = true;
                    Broken?.Invoke(this, EventArgs.Empty);
                    RaiseStatusChanged("⚠️ АВАРИЯ!");
                    continue;
                }

                _currentMaterial--;
                _partsCount++;
                var part = new Part(_partsCount, "Detail-X", 1.5);
                PartProduced?.Invoke(this, part);

                await Task.Delay(_config.ProductionSpeedMs, token);
            }
        }

        private void RaiseStatusChanged(string message)
        {
            StatusChanged?.Invoke(this, message);
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}