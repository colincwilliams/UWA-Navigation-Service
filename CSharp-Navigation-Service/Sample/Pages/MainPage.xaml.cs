using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sample.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : PageBase
    {
        public MainPage()
        {
            this.InitializeComponent();

            /*********************************************************
             * PageBase expects the DataContext to contain a ViewModel
             * when OnNavigatedTo is executed. The DataContext can be
             * set in the constructor, as it is here, or with a XAML
             * binding to a ViewModel locator class.
             *
             * The latter pattern is used with MVVM light.
             *********************************************************/
            this.DataContext = new MainPageViewModel();

            /*********************************************************
             * Optionally override the cache mode for this page. If
             * you have a "home page" that the user will frequently
             * return to, that's a good candidate to make "Required"
             * so that it's always cached.
             *********************************************************/
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void LoadState(NavigationContextBase context, Dictionary<string, object> pageState)
        {
            /*********************************************************
             * Called from the NavigatedTo method for the page.
             * Load your page's previously saved state into the UI
             * using the provided pageState.
             *********************************************************/
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            /*********************************************************
             * Called from the NavigatedFrom method for the page.
             * Save your page's state into the provided pageState
             * object.
             *********************************************************/
        }
    }
}
