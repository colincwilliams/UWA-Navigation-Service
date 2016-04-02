using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Pages
{
    public class Page1ViewModel : ViewModelBase
    {
        private DelegateCommand navigateToMainPageCommand;

        private string value1;
        private string value2;
        private string value3;

        public DelegateCommand NavigateToMainPageCommand
        {
            get
            {
                if (this.navigateToMainPageCommand == null)
                {
                    this.navigateToMainPageCommand = new DelegateCommand((o) =>
                    {
                        App.AppNavigationService.Navigate(
                            typeof(MainPage),
                            new MyNavigationContext()
                            {
                                Value1 = this.Value1,
                                Value2 = this.Value2,
                                Value3 = this.Value3
                            });
                    });
                }

                return this.navigateToMainPageCommand;
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

        public async override Task Activate(NavigationContextBase navigationContext, Dictionary<string, object> pageState)
        {
            await base.Activate(navigationContext, pageState);

            MyNavigationContext context = navigationContext as MyNavigationContext;
            if (pageState != null)
            {
                this.Value1 = pageState[nameof(this.Value1)] as string;
                this.Value2 = pageState[nameof(this.Value2)] as string;
                this.Value3 = pageState[nameof(this.Value3)] as string;
            }
            else if (context != null)
            {
                this.Value1 = context.Value1;
                this.Value2 = context.Value2;
                this.Value3 = context.Value3;
            }
        }

        public override void Deactivate(Dictionary<string, object> pageState)
        {
            pageState[nameof(this.Value1)] = this.Value1;
            pageState[nameof(this.Value2)] = this.Value2;
            pageState[nameof(this.Value3)] = this.Value3;
        }
    }
}
