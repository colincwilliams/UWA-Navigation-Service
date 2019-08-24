// <copyright file="MainPageViewModel.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SampleCommon
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ColinCWilliams.CSharpNavigationService;

    public class MainPageViewModel : ViewModelBase
    {
        private DelegateCommand navigateToPage1Command;

        private string value1;
        private string value2;
        private string value3;

        public DelegateCommand NavigateToPage1Command
        {
            get
            {
                if (this.navigateToPage1Command == null)
                {
                    this.navigateToPage1Command = new DelegateCommand((o) =>
                    {
                        MyNavigationContext context = new MyNavigationContext()
                        {
                            Value1 = this.Value1,
                            Value2 = this.Value2,
                            Value3 = this.Value3,
                        };

                        this.NavigationService.Navigate(typeof(Page1), context);
                    });
                }

                return this.navigateToPage1Command;
            }
        }

        public string Value1
        {
            get { return this.value1; }
            set { this.SetPropertyValue(ref this.value1, value); }
        }

        public string Value2
        {
            get { return this.value2; }
            set { this.SetPropertyValue(ref this.value2, value); }
        }

        public string Value3
        {
            get { return this.value3; }
            set { this.SetPropertyValue(ref this.value3, value); }
        }

        public async override Task Activate(INavigationService navigationService, NavigationContextBase navigationContext, IReadOnlyPageState pageState)
        {
            await base.Activate(navigationService, navigationContext, pageState);

            MyNavigationContext context = navigationContext as MyNavigationContext;
            if (pageState != null)
            {
                /*********************************************************
                 * If you store state on deactivation and it is provided
                 * here, you know that the user has used the back button
                 * or returned from app suspension and should restore state.
                 *********************************************************/
                this.Value1 = pageState[nameof(this.Value1)] as string;
                this.Value2 = pageState[nameof(this.Value2)] as string;
                this.Value3 = pageState[nameof(this.Value3)] as string;
            }
            else if (context != null)
            {
                /*********************************************************
                 * Alternatively, if the user has navigated to the page
                 * for the first time, there won't be existing page state
                 * to restore. The context can be used to populate the page.
                 *********************************************************/
                this.Value1 = context.Value1;
                this.Value2 = context.Value2;
                this.Value3 = context.Value3;
            }
        }

        public override void Deactivate(IDictionary<string, object> pageState)
        {
            base.Deactivate(pageState);

            /*********************************************************
             * Use the deactivate method to save the page's state
             * if desired. This can be used to restore state when the
             * user uses the back button or after app suspension.
             *
             * Note that this page state will also be persisted to disk
             * in the case of suspension, so be aware of how much data
             * you are saving.
             *********************************************************/
            pageState[nameof(this.Value1)] = this.Value1;
            pageState[nameof(this.Value2)] = this.Value2;
            pageState[nameof(this.Value3)] = this.Value3;
        }
    }
}
