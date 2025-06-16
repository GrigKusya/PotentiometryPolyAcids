using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KarginScales.Models;

public class Polymer : INotifyPropertyChanged
{
    private string _name;
    private double _minT;
    private double _maxT;
    private ObservableCollection<DataPoint> _data;

    public ObservableCollection<DataPoint> MeasuredData { get; } = new ObservableCollection<DataPoint>();

    public Polymer(string name, List<DataPoint> data)
    {
        _name = name;
        _data = new ObservableCollection<DataPoint>(data);
        _minT = 0.1; // Фиксированное минимальное значение
        _maxT = data.Max(p => p.Temperature);
    }

    public bool AddDataPoint(double temperature, double gamma)
    {
        temperature = Math.Max(0.1, temperature); // Гарантируем минимум 0.1
        var dataPoint = new DataPoint(Math.Round(temperature, 1), gamma);

        if (MeasuredData.Contains(dataPoint))
            return false;

        for(int i = 0; i < MeasuredData.Count; ++i)
        {
            if (dataPoint.Temperature < MeasuredData[i].Temperature)
            {
                MeasuredData.Insert(i, dataPoint);
                return true;
            }
        }

        MeasuredData.Add(dataPoint);
        return true;
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public double MinT
    {
        get => _minT;
        set
        {
            _minT = value;
            OnPropertyChanged(nameof(_minT));
        }
    }

    public double MaxT
    {
        get => _maxT;
        set
        {
            _maxT = value;
            OnPropertyChanged(nameof(MaxT));
        }
    }

    public ObservableCollection<DataPoint> Data
    {
        get => _data;
        set
        {
            _data = value;
            OnPropertyChanged(nameof(Data));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName]string propertyName = "")
    {
        if (PropertyChanged != null)
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
