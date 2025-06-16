using KarginScales.ViewModels;
using System;
using System.Windows.Threading;

namespace KarginScales.Models;

public class MeasuringDevice : Notifier
{
    private DispatcherTimer _timer;

    private Polymer? _polymer = null;
    private double _currentTemperature;
    private double _setupTemperature;
    private double _gamma;
    private double _step;

    public MeasuringDevice()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(50); // Уменьшите интервал
        _timer.Tick += OnStartMeasurement;
    }

    public double CurrentTemperature
    {
        get { return _currentTemperature; }
        set
        {
            SetValue(ref _currentTemperature, value, nameof(CurrentTemperature));
        }
    }

    public double SetupTemperature
    {
        get { return _setupTemperature; }
        set
        {
            SetValue(ref _setupTemperature, value, nameof(SetupTemperature));
        }
    }

    public double Gamma
    {
        get { return _gamma; }
        set
        {
            SetValue(ref _gamma, value, nameof(Gamma));
        }
    }

    private void OnStartMeasurement(object? sender, EventArgs e)
    {
        if (Math.Abs(_currentTemperature - _setupTemperature) < 0.01)
        {
            _timer.Stop();
            Gamma = InterpolateGamma();
            OnMeasurementCompleted(CurrentTemperature, Gamma);
            return;
        }

        _currentTemperature += _step * 0.2; // Уменьшите шаг изменения
        Gamma = InterpolateGamma(); // Обновляем Gamma при каждом изменении температуры
    }

    public bool IsRunning
    {
        get { return _timer.IsEnabled; }
    }

    public void StartMeasurement(Polymer? selected, double targetVolume)
    {
        if (_timer.IsEnabled || selected == null)
            return;

        // Начинаем с текущего значения температуры
        SetupTemperature = Math.Max(0.1, targetVolume);
        _step = CurrentTemperature < SetupTemperature ? 0.1 : -0.1;
        _polymer = selected;

        _timer.Start();
        OnPropertyChanged(nameof(IsRunning));
    }

    private double InterpolateGamma()
    {
        if (_polymer?.Data == null || _polymer.Data.Count == 0)
            return 0.0;

        Console.WriteLine($"\nCurrent Temp: {_currentTemperature:F4}"); // Логирование

        for (int i = 0; i < _polymer.Data.Count; i++)
        {
            Console.WriteLine($"Point {i}: T={_polymer.Data[i].Temperature:F4}, G={_polymer.Data[i].Gamma:F4}"); // Логирование

            if (Math.Abs(_polymer.Data[i].Temperature - _currentTemperature) < 0.001)
                return _polymer.Data[i].Gamma;

            if (i > 0 && _polymer.Data[i].Temperature > _currentTemperature)
            {
                var prev = _polymer.Data[i - 1];
                var current = _polymer.Data[i];

                double k = (current.Gamma - prev.Gamma) / (current.Temperature - prev.Temperature);
                double interpolated = k * (_currentTemperature - prev.Temperature) + prev.Gamma;

                Console.WriteLine($"Interpolated: {interpolated:F4}"); // Логирование
                return interpolated;
            }
        }
        return 0.0;
    }

    public event EventHandler<MeasurementCompletedEventArgs>? MeasurementCompleted;

    private void OnMeasurementCompleted(double temperature, double gamma)
    {
        if (MeasurementCompleted != null)
            MeasurementCompleted(this, new MeasurementCompletedEventArgs(temperature, gamma));
    }
}
