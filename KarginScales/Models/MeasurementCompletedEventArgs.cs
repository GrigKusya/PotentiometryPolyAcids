using System;

namespace KarginScales.Models;

public class MeasurementCompletedEventArgs : EventArgs
{
    public double Temperature { get; }
    public double Gamma { get; }

    public MeasurementCompletedEventArgs(double temperature, double gamma)
    {
        Temperature = temperature;
        Gamma = gamma;
    }
}
