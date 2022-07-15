using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LumberCalculator
{
    /// <summary>
    /// Provides a mechanism for implementing view model commands
    /// Author: Unknown (not me)
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private readonly TaskScheduler _sync;
        private readonly Predicate<object> _CanExecuteMethod;
        private readonly Action<object> _ExecuteMethod;

        /// <summary>
        /// Creates a new command with the specified can execute and execute methods.
        /// </summary>
        /// <param name="canExecuteMethod"></param>
        /// <param name="executeMethod"></param>
        public DelegateCommand(Predicate<object> canExecuteMethod, Action<object> executeMethod)
        {
            _sync = TaskScheduler.FromCurrentSynchronizationContext();
            _CanExecuteMethod = canExecuteMethod;
            _ExecuteMethod = executeMethod;
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void CanExecChanged()
        {
            Task.Factory.StartNew(() =>
            {
                EventHandler evt = CanExecuteChanged;
                if (evt != null)
                {
                    evt(this, EventArgs.Empty);
                }
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                _sync
            );
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return _CanExecuteMethod(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _ExecuteMethod(parameter);
        }

    }
}
