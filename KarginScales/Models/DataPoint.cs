namespace KarginScales.Models;

public struct DataPoint
{
    public double Temperature { get; set; }
    public double Gamma { get; set; }

    public DataPoint(double temperature, double gamma)
    {
        Temperature = temperature; 
        Gamma = gamma;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not DataPoint other)
            return false;

        return this.Temperature == other.Temperature;
    }

    public override int GetHashCode()
    {
        return Temperature.GetHashCode() * Gamma.GetHashCode();
    }
}
