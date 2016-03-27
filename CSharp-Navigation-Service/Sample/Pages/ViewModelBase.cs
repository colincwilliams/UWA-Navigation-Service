﻿using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Pages
{
    /*********************************************************
     * All of your ViewModels must implement the
     * INavigatableViewModel interface to work with PageBase.
     *********************************************************/
    public abstract class ViewModelBase : INavigatableViewModel
    {
        private DelegateCommand goBackCommand;
        private DelegateCommand goForwardCommand;

        public DelegateCommand GoBackCommand
        {
            get
            {
                if (this.goBackCommand == null)
                {
                    this.goBackCommand = new DelegateCommand(
                        (o) => App.AppNavigationService.GoBack(),
                        (o) => App.AppNavigationService.CanGoBack());
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
                        (o) => App.AppNavigationService.GoForward(),
                        (o) => App.AppNavigationService.CanGoForward());
                }

                return this.goForwardCommand;
            }
        }

        protected bool IsActive { get; set; }

        /*********************************************************
         * Activate is called during the OnNavigatedTo page event.
         *********************************************************/
        public virtual Task Activate(NavigationContextBase navigationContext)
        {
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
    }
}
