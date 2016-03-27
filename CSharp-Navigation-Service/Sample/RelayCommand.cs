// Helper class provided to make the sample functional and is not
// a core part of the CSharp-Navigation-Service.
//
// Used from: https://msdn.microsoft.com/en-us/magazine/dd419663.aspx

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Sample
{
    public class RelayCommand : ICommand
    {
        #region Fields
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged;
        #endregion // Fields

        #region Constructors
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion // ICommand Members
    }
}


