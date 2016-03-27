using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Pages
{
    public class MainPageViewModel : ViewModelBase, INavigatableViewModel
    {
        private RelayCommand navigateToPage1Command;

        public Task Activate(NavigationContextBase navigationContext)
        {
            // Not expecting a NavigationContext so there is nothing to do.
            return Task.FromResult(false);
        }

        public RelayCommand NavigateToPage1Command
        {
            get
            {
                if (this.navigateToPage1Command == null)
                {
                    this.navigateToPage1Command = new RelayCommand((o) =>
                    {
                        App.AppNavigationService.Navigate(typeof(Page1));
                    });
                }

                return this.navigateToPage1Command;
            }
        }
    }
}
