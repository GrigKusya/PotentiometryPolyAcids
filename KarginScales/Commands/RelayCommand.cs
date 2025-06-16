using System;
using System.Windows.Input;

namespace KarginScales.Commands;

public class RelayCommand : ICommand
{
    protected readonly Action<object?> _execute;
    protected readonly Func<object?, bool>? _canExcecute;

    public RelayCommand(Action<object>? execute, Func<object?, bool>? canExecute = null)
    {
        if (execute == null)
            throw new ArgumentNullException(nameof(execute));
        _execute = execute;
        _canExcecute = canExecute;
    }

    public virtual void Execute(object? parameter) => _execute?.Invoke(parameter);
    public virtual bool CanExecute(object? parameter) => _canExcecute?.Invoke(parameter) ?? true;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
