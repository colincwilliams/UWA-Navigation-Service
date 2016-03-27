using ColinCWilliams.CSharpNavigationService;
using Sample.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private readonly List<Type> knownTypes = new List<Type>
        {
            /*********************************************************
             * The NavigationService needs to know about the types
             * it's going to be serializing when saving state. List
             * them below.
             *
             * There are two categories of types that you will want
             * to list:
             *  1. NavigationContext Sub-Types - These are the objects
             *     inheriting from NavigationContextBase that are
             *     passed during page navigation.
             *
             *  2. Other state types - These are the custom types
             *     that are included in your NavigationContexts or
             *     used when saving the page state.
             *********************************************************/

            // Add NavigationContext sub-types
            typeof(MyNavigationContext),
            // typeof(MyOtherNavigationContext),
            // etc.

            // Add other state types
            // typeof(MyType),
            // typeof(MyOtherType),
            // etc.
        };

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            /*********************************************************
             * PageBase sets the default cache mode for new pages to
             * Enabled, rather than Microsoft's default of Disabled.
             * You can optionally set the default for new pages back
             * to Disabled, or any other value.
             *
             * Note: the cache mode for individual pages can be
             * overidden in their constructor as desired.
             *********************************************************/
            // PageBase.DefaultCacheMode = NavigationCacheMode.Disabled;
        }

        /*********************************************************
         * For this sample the NavigationService is being stored
         * as a static property on the App; however in normal use
         * I would recommend storing this in a IOC system, such
         * as MVVMLight.
         *********************************************************/
        public static INavigationService AppNavigationService { get; private set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Ensure NavigationService knows about all types that may be used in serialization
            NavigationService.AddKnownTypes(this.knownTypes);

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Create the NavigationService for the frame
                App.AppNavigationService = NavigationService.RegisterFrame(rootFrame, "RootFrame");

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    /*********************************************************
                     * Attempt to restore state saved during suspension.
                     *
                     * Note that OnLaunched needs to be async. Because this
                     * creates the application, it works as expected.
                     *********************************************************/
                    try
                    {
                        await NavigationService.RestoreStateAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            try
            {
                /*********************************************************
                 * Save the entire navigation state to disk. This includes
                 * all NavigationContext objects currently in the
                 * navigation stack.
                 *
                 * Note that OnSuspending needs to be async. Because of
                 * the acquired deferral, this will work as expected.
                 *********************************************************/
                await NavigationService.SaveStateAsync();
            }
            catch (Exception)
            {
                // Error while saving state; let's not crash the app in production
                Debug.Assert(false, "Exception thrown while saving state.");
            }

            deferral.Complete();
        }
    }
}
