// <copyright file="ViewModelBase.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SampleCommon
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using ColinCWilliams.UwaNavigationService;

    /*********************************************************
     * All of your ViewModels must implement the
     * INavigatableViewModel interface to work with PageBase.
     *********************************************************/
    public abstract class ViewModelBase : INavigatableViewModel, INotifyPropertyChanged
    {
        private DelegateCommand goBackCommand;
        private DelegateCommand goForwardCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand GoBackCommand
        {
            get
            {
                if (this.goBackCommand == null)
                {
                    this.goBackCommand = new DelegateCommand(
                        (o) => this.NavigationService?.GoBack(),
                        (o) => this.NavigationService?.CanGoBack);
                }

                return this.goBackCommand;
            }
        }

        public DelegateCommand GoForwardCommand
        {
            get
            {
                if (this.goForwardCommand == null)
                {
                    this.goForwardCommand = new DelegateCommand(
                        (o) => this.NavigationService?.GoForward(),
                        (o) => this.NavigationService?.CanGoForward);
                }

                return this.goForwardCommand;
            }
        }

        protected INavigationService NavigationService { get; private set; }

        protected bool IsActive { get; set; }

        /*********************************************************
         * Activate is called during the OnNavigatedTo page event.
         *********************************************************/
        public virtual Task Activate(INavigationService navigationService, NavigationContextBase navigationContext, IReadOnlyPageState pageState)
        {
            this.NavigationService = navigationService;

            /*********************************************************
             * If you use the CanGoBack() or CanGoForward() methods
             * as a DelegateCommand's CanExecute method, you must
             * tell the button when navigation has completed,
             * otherwise the button will be permanenty disabled.
             *
             * Here in the Activate method is a good place to do that.
             *********************************************************/
            this.GoBackCommand.RaiseCanExecuteChanged();
            this.GoForwardCommand.RaiseCanExecuteChanged();

            return Task.FromResult(false);
        }

        public virtual void Deactivate(IDictionary<string, object> pageState)
        {
            // Nothing to do
        }

        protected void SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
