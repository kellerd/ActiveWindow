using System;
using System.Windows.Input;
public class RelayCommand : RelayCommand<object>
{
    public RelayCommand(Action<object> execute)
        : this(execute, DefaultCanExecute)
    {  }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        : base(execute, canExecute)
    { }
}
public class RelayCommand<T> : ICommand
{
    private Action<T> execute;

    private Predicate<T> canExecute;

    private event EventHandler CanExecuteChangedInternal;

    public RelayCommand(Action<T> execute)
        : this(execute, DefaultCanExecute)
    {
    }

    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
        if (execute == null)
        {
            throw new ArgumentNullException("execute");
        }

        if (canExecute == null)
        {
            throw new ArgumentNullException("canExecute");
        }

        this.execute = execute;
        this.canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
            this.CanExecuteChangedInternal += value;
        }

        remove
        {
            CommandManager.RequerySuggested -= value;
            this.CanExecuteChangedInternal -= value;
        }
    }

    public bool CanExecute(object parameter)
    {
        return this.canExecute != null && this.canExecute((T)(parameter));
    }

    public void Execute(object parameter)
    {
        this.execute((T)parameter);
    }

    public void OnCanExecuteChanged()
    {
        EventHandler handler = this.CanExecuteChangedInternal;
        if (handler != null)
        {
            //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
            handler.Invoke(this, EventArgs.Empty);
        }
    }

    public void Destroy()
    {
        this.canExecute = _ => false;
        this.execute = _ => { return; };
    }

    protected static bool DefaultCanExecute(T parameter)
    {
        return true;
    }
}