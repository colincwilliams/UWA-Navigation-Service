// <copyright file="DummyPage.xaml.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

/*
    At least one page required in the project being run, even if it isn't used,
    otherwise Frame.Navigate fails with an AccessViolationException.

    Please ignore this file.

    Source of reasoning: http://danielvaughan.org/post/UWP-AccessViolationException-when-Navigating-to-a-Page-in-Another-Assembly.aspx
*/

namespace Sample
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DummyPage : Page
    {
        public DummyPage()
        {
            this.InitializeComponent();
        }
    }
}
