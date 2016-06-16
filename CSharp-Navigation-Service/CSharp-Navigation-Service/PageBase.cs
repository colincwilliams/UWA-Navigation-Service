// <copyright file="PageBase.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A base class that all pages should inherit from for navigation, state
    /// saving and other essential functionality.
    /// </summary>
    public abstract class PageBase : Page
    {
        private static NavigationCacheMode defaultCacheMode = NavigationCacheMode.Enabled;

        private INavigationContextService contextService;
        private INavigationService navigationService;
        private string pageKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBase" /> class.
        /// </summary>
        protected PageBase()
        {
            this.NavigationCacheMode = DefaultCacheMode;

            this.Loaded += this.PageBase_Loaded;
            this.Unloaded += this.PageBase_Unloaded;
        }

        /// <summary>
        /// Gets or sets the cache mode set by default on new pages.
        /// </summary>
        public static NavigationCacheMode DefaultCacheMode
        {
            get { return defaultCacheMode; }
            set { defaultCacheMode = value; }
        }

        /// <summary>
        /// Gets the current ViewModel for the page.
        /// </summary>
        protected INavigatableViewModel ViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the page is navigated to. Initializes properties, such as
        /// the navigation service and view model, as well as restores context
        /// from the suspension manager.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e == null)
            {
                return;
            }

            // Get NavigationService
            if (this.navigationService == null)
            {
                this.navigationService = NavigationService.GetNavigationService(this.Frame);
                this.contextService = this.navigationService.ContextService;
            }

            // Set ViewModel
            if (this.ViewModel == null)
            {
                INavigatableViewModel viewModel = this.DataContext as INavigatableViewModel;
                if (viewModel == null)
                {
                    throw new InvalidOperationException("Viewmodel must implement IActivatableViewModel interface.");
                }

                this.ViewModel = viewModel;
            }

            // Load page state if this page wasn't already cached by the frame
            var frameState = SuspensionManager.Instance.SessionStateForFrame(this.Frame);
            this.pageKey = "Page-" + this.Frame.BackStackDepth;

            // Get the NavigationContext
            NavigationContextBase context = this.GetContextFromParameter(e.Parameter);

            Dictionary<string, object> pageState = null;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page to the
                // navigation stack
                var nextPageKey = this.pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }
            }
            else
            {
                // Load page state using the same strategy for loading suspended state and
                // recreating pages discarded from cache
                pageState = (Dictionary<string, object>)frameState[this.pageKey];
            }

            // Activate the ViewModel
            await this.ViewModel.Activate(context, pageState);
        }

        /// <summary>
        /// Called when the page is navigated away from, saving the state of the page for suspension.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Dictionary<string, object> frameState = SuspensionManager.Instance.SessionStateForFrame(this.Frame);

            var pageState = new Dictionary<string, object>();
            this.ViewModel.Deactivate(pageState);

            frameState[this.pageKey] = pageState;
        }

        private static bool IsParameterNull(object parameter)
        {
            // If it's directly null, we can return.
            if (parameter == null)
            {
                return true;
            }

            // It may be an empty string on launch, so if it's a string check if it's empty.
            // If it is empty, that counts as null.
            string strParameter = parameter as string;

            if (strParameter != null && string.IsNullOrWhiteSpace(strParameter))
            {
                return true;
            }

            // It's not null.
            return false;
        }

        private void PageBase_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#else
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= this.CoreDispatcher_AcceleratorKeyActivated;
            Window.Current.CoreWindow.PointerPressed -= this.CoreWindow_PointerPressed;
#endif
        }

        private void PageBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#else
            // Keyboard and mouse navigation only apply when occupying the entire window
            if (this.ActualHeight == Window.Current.Bounds.Height &&
                this.ActualWidth == Window.Current.Bounds.Width)
            {
                // Listen to the window directly so focus isn't required
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += this.CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += this.CoreWindow_PointerPressed;
            }
#endif
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Invoked when the hardware back button is pressed. For Windows Phone only.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.navigationService.CanGoBack())
            {
                e.Handled = true;
                this.navigationService.GoBack();
            }
        }
#else

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations, when
        /// this page is active and occupies the entire window.  Used to detect keyboard navigation
        /// between pages even when the page itself doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    e.Handled = true;
                    this.navigationService.GoBack();
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    e.Handled = true;
                    this.navigationService.GoForward();
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        /// page is active and occupies the entire window.  Used to detect browser-style next and
        /// previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed ||
                properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed)
            {
                return;
            }

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;

                if (backPressed)
                {
                    this.navigationService.GoBack();
                }

                if (forwardPressed)
                {
                    this.navigationService.GoForward();
                }
            }
        }
#endif

        private NavigationContextBase GetContextFromParameter(object parameter)
        {
            NavigationContextBase context = null;

            if (!IsParameterNull(parameter))
            {
                bool isLong = parameter is long;
                Debug.Assert(isLong, "Navigation parameter must be null or a long.");

                if (isLong)
                {
                    context = this.contextService.Get((long)parameter);
                }
            }

            return context;
        }
    }
}