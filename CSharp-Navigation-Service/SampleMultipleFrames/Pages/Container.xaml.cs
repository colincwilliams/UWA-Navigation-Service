// <copyright file="Container.xaml.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SampleMultipleFrames
{
    using ColinCWilliams.CSharpNavigationService;
    using SampleCommon;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Container : Page
    {
        public Container()
        {
            this.InitializeComponent();

            this.Loaded += this.Container_Loaded;
        }

        private void Container_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.RegisterFrame(this.FrameLeft, typeof(MainPage));
            NavigationService.RegisterFrame(this.FrameRight, typeof(MainPage));
        }
    }
}
