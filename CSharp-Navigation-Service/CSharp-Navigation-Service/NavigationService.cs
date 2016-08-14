// <copyright file="NavigationService.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A navigation service which facilitates Frame navigation and suspension
    /// with passing of complex types.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private static readonly INavigationServiceManager ServiceManager = new NavigationServiceManager(new SuspensionManager());
        private readonly INavigationContextService contextService;
        private readonly WeakReference<Frame> rootFrame;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="frame">The Frame to use for navigation.</param>
        /// <param name="contextService">The context service to use for storing and retrieving Navigation Contexts during navigation.</param>
        internal NavigationService(Frame frame, INavigationContextService contextService)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            if (contextService == null)
            {
                throw new ArgumentNullException(nameof(contextService));
            }

            this.contextService = contextService;
            this.rootFrame = new WeakReference<Frame>(frame);
            this.name = frame.Name;
            this.PageStates = new Dictionary<string, PageState>();
        }

        /// <summary>
        /// Gets a value indicating whether calling <see cref="GoBack" /> will success.
        /// </summary>
        /// <returns>True if back navigation can occur, false otherwise.</returns>
        public bool CanGoBack => this.RootFrame.CanGoBack;

        /// <summary>
        /// Gets a value indicating whether calling <see cref="GoForward" /> will success.
        /// </summary>
        /// <returns>True if forward navigation can occur, false otherwise.</returns>
        public bool CanGoForward => this.RootFrame.CanGoForward;

        /// <summary>
        /// Gets the states for the individual pages this Navigation Service has seen.
        /// </summary>
        internal Dictionary<string, PageState> PageStates { get; private set; }

        /// <summary>
        /// Gets the context service associated with this NavigationService.
        /// </summary>
        internal INavigationContextService ContextService
        {
            get { return this.contextService; }
        }

        /// <summary>
        /// Gets the root frame for the navigation service.
        /// </summary>
        internal Frame RootFrame
        {
            get
            {
                Frame frame = this.GetFrameSafe();
                if (frame == null)
                {
                    throw new InvalidOperationException("NavigationService still in use after Frame has been garbage collected.");
                }

                return frame;
            }
        }

        /// <summary>
        /// Gets the name of this NavigationService.
        /// </summary>
        internal string Name
        {
            get
            {
                if (this.GetFrameSafe().Name != this.name)
                {
                    throw new InvalidOperationException("Frame's name changed after this NavigationService was constructed.");
                }

                return this.name;
            }
        }

        /// <summary>
        /// Creates a new NavigationService for a Frame, registering the frame and NavigationService
        /// for future retrieval and registering the frame with the NavigationContextService.
        /// </summary>
        /// <param name="frame">The frame to register.</param>
        /// <param name="defaultPage">The default page to navigate to if there is no state to restore.</param>
        /// <param name="defaultNavigationContext">The navigation context to provide on navigation to defaultPage.</param>
        /// <param name="restoreState">True to attempt to restore previously saved state, false to just navigate to defaultPage
        ///     without attempting to restore state.</param>
        /// <returns>The NavigationService for the registered frame.</returns>
        public static INavigationService RegisterFrame(Frame frame, Type defaultPage, NavigationContextBase defaultNavigationContext = null, bool restoreState = true)
        {
            return ServiceManager.RegisterFrame(frame, defaultPage, defaultNavigationContext, restoreState);
        }

        /// <summary>
        /// Unregisters a Frame and its NavigationService, removing all references to both the
        /// frame and navigation service. Unregisters the frame from the NavigationContextService.
        /// </summary>
        /// <param name="frame">The frame to unregister.</param>
        public static void UnregisterFrame(Frame frame)
        {
            ServiceManager.UnregisterFrame(frame);
        }

        /// <summary>
        /// Retrieves the NavigationService for a registered Frame.
        /// </summary>
        /// <param name="frame">The Frame to get the NavigationService for.</param>
        /// <returns>The NavigationService for the provided frame.</returns>
        public static INavigationService GetNavigationService(Frame frame)
        {
            return ServiceManager.GetNavigationService(frame);
        }

        /// <summary>
        /// Saves the current navigation state in preparation for
        /// app suspension.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        public static async Task SaveStateAsync()
        {
            await ServiceManager.SaveStateAsync();
        }

        /// <summary>
        /// Restores the current navigation state from disk.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        public static async Task RestoreStateAsync()
        {
            await ServiceManager.RestoreStateAsync();
        }

        /// <summary>
        /// Adds the provided types to a known type list so that they
        /// can be serialized when <see cref="SaveStateAsync" /> is called.
        /// </summary>
        /// <param name="types">The known types to add.</param>
        public static void AddKnownTypes(List<Type> types)
        {
            ServiceManager.AddKnownTypes(types);
        }

        /// <summary>
        /// Navigate to the specified page with the provided context.
        /// </summary>
        /// <param name="pageType">The type of page to navigate to.</param>
        /// <param name="context">The context that should be provided to the page after navigation.</param>
        public void Navigate(Type pageType, NavigationContextBase context = null)
        {
            if (pageType == null)
            {
                throw new ArgumentNullException(nameof(pageType));
            }

            if (context != null && !ServiceManager.KnownTypes.Contains(context.GetType()))
            {
                throw new ArgumentException(
                    "Cannot navigate with a context that has not been registered as a Known Type. Use NavigationService.AddKnownTypes to register this context type before use.",
                    "context");
            }

            object parameter = null;

            if (context != null)
            {
                parameter = this.contextService.Add(context);
            }

            this.RootFrame.Navigate(pageType, parameter);
        }

        /// <summary>
        /// Navigate back a page in the back stack.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                this.RootFrame.GoBack();
            }
        }

        /// <summary>
        /// Navigate forward a page in the forward stack.
        /// </summary>
        public void GoForward()
        {
            if (this.CanGoForward)
            {
                this.RootFrame.GoForward();
            }
        }

        /// <summary>
        /// Saves the state of this NavigationService.
        /// </summary>
        /// <returns>The saved state.</returns>
        internal FrameState SaveState()
        {
            return new FrameState()
            {
                Navigation = this.RootFrame.GetNavigationState(),
                ContextService = this.ContextService.SaveState(),
                PageStates = this.PageStates
            };
        }

        /// <summary>
        /// Restores the state of this Navigation Service from the <see cref="SuspensionManager"/>.
        /// </summary>
        /// <param name="state">The state to restore from.</param>
        internal void RestoreState(FrameState state)
        {
            if (state != null)
            {
                this.ContextService.RestoreState(state.ContextService);
                this.PageStates = state.PageStates;

                // SetNavigationState last so that everything else is restored.
                // This triggers OnNavigatedTo call for the page.
                this.RootFrame.SetNavigationState(state.Navigation);
            }
        }

        /// <summary>
        /// Attempts to get the Frame from the weak reference.
        /// </summary>
        /// <returns>The frame or null if the Frame was garbage collected.</returns>
        internal Frame GetFrameSafe()
        {
            Frame frame = null;
            this.rootFrame.TryGetTarget(out frame);

            return frame;
        }
    }
}
