// <copyright file="MainPage.xaml.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SampleCommon
{
    using ColinCWilliams.CSharpNavigationService;
    using Windows.UI.Xaml.Navigation;

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
    }
}
