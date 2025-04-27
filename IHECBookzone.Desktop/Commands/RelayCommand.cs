using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IHECBookzone.Desktop.Commands
{
    /// <summary>
    /// A command implementation that can be used across ViewModels, with support for async operations
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Func<object, Task> _executeAsync;
        private readonly Predicate<object> _canExecute;
        
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
                
            _executeAsync = param => { execute(param); return Task.CompletedTask; };
            _canExecute = canExecute;
        }
        
        public RelayCommand(Func<object, Task> executeAsync, Predicate<object> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        
        public async void Execute(object parameter)
        {
            await _executeAsync(parameter);
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        
        /// <summary>
        /// Raises the CanExecuteChanged event to signal a change in command execution status
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
} 