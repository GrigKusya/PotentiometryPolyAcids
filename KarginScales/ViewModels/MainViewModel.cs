using KarginScales.Service;
using System;
using KarginScales.Models;
using KarginScales.Commands;
using System.Collections.Generic;
using System.ComponentModel;

namespace KarginScales.ViewModels;

public class MainViewModel : Notifier
{
    #region Fields

    private IDialogService _dialogDataService;
    private IDataService _dataService;

    private double _currentTemperature;
    private double _setupTemperature;
    private double _lastMeasuredVolume = 0.1;
    private double _gamma;
    private MeasuringDevice _device;
    private Polymer _selectedPolymer;
    private ChartViewModel _plot;
    private double _lastUsedVolume = 0.1; // Хранит предыдущее значение объёма

    #endregion

    #region Initialize
    public MainViewModel(IDialogService dialogDataService, IDataService dataService)
    {
        string pathFile = "Content\\D.xlsx";
        _dialogDataService = dialogDataService;
        _dataService = dataService;

        var result = _dataService.LoadData(pathFile);

        if (result.IsSuccess)
            Polymers = result.Data;
        else
        {
            _dialogDataService.ShowMessage(result.ErrorMessage, "Проверьте подключение");
            Polymers = new List<Polymer>();
        }

        _device = new MeasuringDevice();
        _device.PropertyChanged += DeviceOnPropertyChanged;
        _device.MeasurementCompleted += OnMeasurementCompleted;

        _plot = new ChartViewModel();
    }

    private void DeviceOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case nameof(_device.CurrentTemperature):
                _lastUsedVolume = _device.CurrentTemperature; // Сохраняем текущее значение
                CurrentTemperature = _lastUsedVolume;
                break;
            case nameof(_device.Gamma):
                Gamma = _device.Gamma;
                break;
            case nameof(_device.IsRunning):
                _startMeasurement.RaiseCanExecuteChanged();
                break;
        }
    }

    private void OnMeasurementCompleted(object? sender, MeasurementCompletedEventArgs e)
    {
        if (SelectedPolymer == null) return;

        CurrentTemperature = e.Temperature; // Обновляем только после измерения
        SelectedPolymer.AddDataPoint(e.Temperature, e.Gamma);
    }

    #endregion

    #region Properties
    public List<Polymer>? Polymers { get; }

    public double CurrentTemperature
    {
        get => _lastMeasuredVolume;
        set
        {
            _lastMeasuredVolume = Math.Max(0.1, value);
            OnPropertyChanged(nameof(CurrentTemperature));
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

    private string _password;
    public string Password
    {
        get { return _password; }
        set
        {
            SetValue(ref _password, value, nameof(Password));
        }
    }

    public ChartViewModel Plot
    {
        get { return _plot; }
        set
        {
            SetValue(ref _plot, value, nameof(Plot));
        }
    }

    public Polymer SelectedPolymer
    {
        get { return _selectedPolymer; }
        set
        {
            SetValue(ref _selectedPolymer, value, nameof(SelectedPolymer));
            SetupTemperature = _lastUsedVolume; // Устанавливаем последнее сохранённое значение
            Gamma = 0.0;
            Plot.UpdateChart(SelectedPolymer);
        }
    }
    #endregion

    #region Commands

    private RelayCommand _raiseTemp;
    private RelayCommand _lowerTemp;
    private RelayCommand _startMeasurement;
    private RelayCommand _showTeacherChart;
    private RelayCommand _hiddenTeacherChart;

    public RelayCommand RaiseTemp
    {
        get
        {
            return _raiseTemp ?? (_raiseTemp = new RelayCommand(
            o => SetupTemperature = Math.Round(SetupTemperature + 0.1, 1),
            o => SelectedPolymer != null && (SetupTemperature + 0.1) <= SelectedPolymer.MaxT));
        }
    }

    public RelayCommand LowerTemp
    {
        get
        {
            return _lowerTemp ?? (_lowerTemp = new RelayCommand(
            o => SetupTemperature = Math.Round(Math.Max(0.1, SetupTemperature - 0.1), 1),
            o => SelectedPolymer != null && SetupTemperature > 0.1));
        }
    }

    public RelayCommand StartMeasurement
    {
        get
        {
            return _startMeasurement ??
                (_startMeasurement = new RelayCommand(OnStartMeasurement, o => !_device.IsRunning));
        }
    }

    public RelayCommand ShowTeacherChart
    {
        get
        {
            return _showTeacherChart ??
                (_showTeacherChart = new RelayCommand(OnShowTeacherChart));
        }
    }

    public RelayCommand HiddenTeachetChart
    {
        get
        {
            return _hiddenTeacherChart ??
                (_hiddenTeacherChart = new RelayCommand(OnHiddenTeacherChart));
        }
    }

    private void OnStartMeasurement(object p)
    {
        // Передаем только выбранный полимер и целевой объем (2 параметра)
        _device.StartMeasurement(SelectedPolymer, SetupTemperature);
    }

    private void OnShowTeacherChart(object p)
    {
        if (p is string password)
            Plot.ShowTeacherChart(password);
        OnPropertyChanged(nameof(Plot));
    }

    private void OnHiddenTeacherChart(object p)
    {
        Plot.HiddenTeacherChart();
        OnPropertyChanged(nameof(Plot));

        Password = String.Empty;
    }

    #endregion
}