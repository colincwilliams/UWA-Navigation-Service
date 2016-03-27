using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Pages
{
    public abstract class ViewModelBase
    {
        private RelayCommand goBackCommand;

        public RelayCommand GoBackCommand
        {
            get
            {
                if (this.goBackCommand == null)
                {
                    this.goBackCommand = new RelayCommand(
                        (o) =>
                        {
                            App.AppNavigationService.GoBack();
                        },
                        (o) => { return App.AppNavigationService.CanGoBack(); });
                }

                return this.goBackCommand;
            }
        }
    }
}
