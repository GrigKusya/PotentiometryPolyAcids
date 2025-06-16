using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace KarginScales.Views;

public partial class LCDData : UserControl
{
    public static readonly DependencyProperty DataProperty;
    public static readonly DependencyProperty TitleProperty;
    public static readonly DependencyProperty TextForegroundProperty;

    static LCDData()
    {
        FrameworkPropertyMetadata metadataData = new FrameworkPropertyMetadata(default(double));
        DataProperty = DependencyProperty.Register("Data", typeof(double), typeof(LCDData), metadataData);

        TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LCDData));
        TextForegroundProperty = DependencyProperty.Register("TextForeground", typeof(Brush), typeof(LCDData), new PropertyMetadata(Brushes.Black, OnTextBoxForegroundChanged));
    }

    public double Data
    {
        get => (double)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Brush TextForeground
    {
        get => (Brush)GetValue(TextForegroundProperty);
        set => SetValue(TextForegroundProperty, value);
    }

    private static void OnTextBoxForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as LCDData;
        if (control != null)
        {
            control.dataTextBox.Foreground = (Brush)e.NewValue;
        }
    }

    public LCDData()
    {
        InitializeComponent();
    }
}
