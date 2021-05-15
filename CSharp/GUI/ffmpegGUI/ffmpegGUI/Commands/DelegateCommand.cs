using System;
using System.Windows.Input;

namespace ffmpegGUI.Commands
{
    /// <summary>
    /// Delegate Command
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region private member
        Action<object> _execute;
        Func<object, bool> _canExecute;
        #endregion

        #region command methods
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
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
            _canExecute = canExecute;
        }
        #endregion

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
