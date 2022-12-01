using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ChatClient.Infra
{
    public class RelayCommand<T> : ICommand, IDisposable
    {
        private Action<T> _execute;
        private Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null) { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            Predicate<T> canExecute = _canExecute;

            if (canExecute == null)
                return true;

            bool result = false;
            try
            {
                result = canExecute((T)parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on RelayCommand.CanExcecute :{ex.Message}");
            }
            return result;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            Action<T> execute = _execute;
            try
            {
                if (execute == null)
                    return;
                execute((T)parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on RelayCommand.Excecute :{ex.Message}");
            }
        }

        public void Dispose()
        {
            this._execute = (Action<T>)null;
            this._canExecute = (Predicate<T>)null;
        }
    }
}
