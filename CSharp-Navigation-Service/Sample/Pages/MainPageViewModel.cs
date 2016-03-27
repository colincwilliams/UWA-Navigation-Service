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

        public DelegateCommand NavigateToPage1Command
        {
            get
            {
                if (this.navigateToPage1Command == null)
                {
                    this.navigateToPage1Command = new DelegateCommand((o) =>
                    {
                        App.AppNavigationService.Navigate(typeof(Page1));
                    });
                }

                return this.navigateToPage1Command;
            }
        }
    }
}
