using KarginScales.Service;
using System.Windows;

namespace KarginScales.Views;

public class MessageBoxDialogService : IDialogService
{
    public void ShowMessage(string message, string title = "Сообщение")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
