using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Pages
{
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
                        App.AppNavigationService.Navigate(
                            typeof(Page1),
                            new MyNavigationContext()
                            {
                                Value1 = this.Value1,
                                Value2 = this.Value2,
                                Value3 = this.Value3
                            });
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

        public async override Task Activate(NavigationContextBase navigationContext)
        {
            await base.Activate(navigationContext);

            if (IsActive)
            {
                return;
            }

            MyNavigationContext context = navigationContext as MyNavigationContext;
            if (context != null)
            {
                this.Value1 = context.Value1;
                this.Value2 = context.Value2;
                this.Value3 = context.Value3;
            }

            this.IsActive = true;
        }
    }
}
