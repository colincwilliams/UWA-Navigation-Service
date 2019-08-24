// <copyright file="Page1.xaml.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SampleCommon
{
    using ColinCWilliams.CSharpNavigationService;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page1Base
    {
        public Page1()
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
            this.DataContext = new Page1ViewModel();
        }
    }

    public abstract class Page1Base : PageBase<Page1ViewModel>
    {
    }
}
