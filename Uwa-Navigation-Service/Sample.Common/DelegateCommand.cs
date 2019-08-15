// <copyright file="DelegateCommand.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

// Helper class provided to make the sample functional and is not
// a core part of the CSharp-Navigation-Service.
//
// Modified from: http://stackoverflow.com/a/11964495
namespace SampleCommon
{
    using System;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool?> canExecuteMethod;
        private readonly Action<object> executeMethod;

        public DelegateCommand(Action<object> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<object> executeMethod, Func<object, bool?> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return (this.canExecuteMethod == null) || this.canExecuteMethod(parameter).GetValueOrDefault();
        }

        public void Execute(object parameter)
        {
            if (this.executeMethod != null)
            {
                this.executeMethod(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}