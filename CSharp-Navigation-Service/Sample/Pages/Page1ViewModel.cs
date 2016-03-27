using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Pages
{
    public class Page1ViewModel : ViewModelBase, INavigatableViewModel
    {
        private RelayCommand navigateToMainPageCommand;

        public Task Activate(NavigationContextBase navigationContext)
        {
            // No navigationContext expected, so nothing to do
            return Task.FromResult(false);
        }

        public RelayCommand NavigateToMainPageCommand
        {
            get
            {
                if (this.navigateToMainPageCommand == null)
                {
                    this.navigateToMainPageCommand = new RelayCommand((o) =>
                    {
                        App.AppNavigationService.Navigate(typeof(MainPage));
                    });
                }

                return this.navigateToMainPageCommand;
            }
        }
    }
}
