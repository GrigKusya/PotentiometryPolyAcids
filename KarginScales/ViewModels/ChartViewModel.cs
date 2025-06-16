using KarginScales.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Linq;

namespace KarginScales.ViewModels;

public class ChartViewModel : Notifier
{
    private ObservableCollection<ISeries> _series;
    private ObservableCollection<ICartesianAxis> _xAxis;
    private ObservableCollection<ICartesianAxis> _yAxis;
    private string _password = "ВМС";

    public ChartViewModel()
    {
        _series = new ObservableCollection<ISeries>()
        {
            new LineSeries<DataPoint>
            {
                Values = new ObservableCollection<DataPoint>(),
                Mapping = (point, index) => new Coordinate(point.Temperature, point.Gamma),
                Fill = null,
                IsVisible = false,
            },

            new LineSeries<DataPoint>
            {
                Values = new ObservableCollection<DataPoint>(),
                Mapping = (point, index) => new Coordinate(point.Temperature, point.Gamma),
                Fill = null
            }
        };

        _xAxis = new ObservableCollection<ICartesianAxis>()
        {
            new Axis
            {
                Name = "Объём титранта, мл", // Было "Температура, °C"
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                Labeler = value => value.ToString("F1") // Округление до десятых
            }
        };

        _yAxis = new ObservableCollection<ICartesianAxis>()
        {
            new Axis
            {
                Name = "pH",
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };
    }

    public ObservableCollection<ISeries> Series
    {
        get { return _series; }
        set
        {
            SetValue(ref _series, value, nameof(Series));
        }
    }

    public ObservableCollection<ICartesianAxis> XAxis
    {
        get { return _xAxis; }
        private set { SetValue(ref _xAxis, value, nameof(XAxis)); }
    }

    public ObservableCollection<ICartesianAxis> YAxis
    {
        get { return _yAxis; }
        private set { SetValue(ref _yAxis, value, nameof(YAxis)); }
    }

    public void ShowTeacherChart(string password)
    {
        if (password == _password)
        {
            Series[0].IsVisible = true;
            OnPropertyChanged(nameof(Series));
        }
    }

    public void HiddenTeacherChart()
    {
        Series[0].IsVisible = false;
        OnPropertyChanged(nameof(Series));
    }

    public void UpdateChart(Polymer selected)
    {
        if (selected == null)
            return;

        var dataForTeacher = Series[0] as LineSeries<DataPoint>;
        var measuredData = Series[1] as LineSeries<DataPoint>;

        if (dataForTeacher != null)
        {
            dataForTeacher.Values = selected.Data;

            XAxis[0].MinLimit = dataForTeacher.Values.Min(p => p.Temperature - 1);
            XAxis[0].MaxLimit = dataForTeacher.Values.Max(p => p.Temperature + 1);

            YAxis[0].MinLimit = dataForTeacher.Values.Min(p => p.Gamma - 1);
            YAxis[0].MaxLimit = dataForTeacher.Values.Max(p => p.Gamma + 1);
        }

        if (measuredData != null)
            measuredData.Values = selected.MeasuredData;

        OnPropertyChanged(nameof(Series));
    }
}

