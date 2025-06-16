using KarginScales.Service;
using KarginScales.ViewModels;
using System.Windows;


namespace KarginScales.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel(new MessageBoxDialogService(), new ExcelDataService());
    }

    private void LCDData_Loaded()
    {

    }
}