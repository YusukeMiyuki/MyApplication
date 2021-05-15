using System;
using System.Windows;
using System.Windows.Input;

namespace ffmpegGUI.Commands
{
    /// <summary>
    /// Delegate Command
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region private member
        readonly Action<object> _execute;
        readonly Func<object, bool> _canExecute;
        #endregion

        #region command methods
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter) => _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="execute">execute action</param>
        /// <param name="canExecute">canExecute Func</param>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? ((param) => { return true; });
        }
        #endregion

        //public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
