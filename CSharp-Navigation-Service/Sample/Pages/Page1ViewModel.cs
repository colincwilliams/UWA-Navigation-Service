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

        public DelegateCommand NavigateToMainPageCommand
        {
            get
            {
                if (this.navigateToMainPageCommand == null)
                {
                    this.navigateToMainPageCommand = new DelegateCommand((o) =>
                    {
                        App.AppNavigationService.Navigate(typeof(MainPage));
                    });
                }

                return this.navigateToMainPageCommand;
            }
        }
    }
}
