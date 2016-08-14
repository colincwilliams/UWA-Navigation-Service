// <copyright file="NavigationServiceManager.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Manages all registered NavigationServices and provides support
    /// for suspending and restoring them.
    /// </summary>
    internal class NavigationServiceManager : INavigationServiceManager
    {
        private static readonly List<NavigationService> NavigationServices = new List<NavigationService>();

        private ISuspensionManager suspensionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationServiceManager"/> class.
        /// </summary>
        /// <param name="suspensionManager">The suspension manager to use.</param>
        internal NavigationServiceManager(ISuspensionManager suspensionManager)
        {
            this.suspensionManager = suspensionManager;
        }

        /// <summary>
        /// Gets a readonly Collection of custom types provided to the <see cref="DataContractSerializer"/> when
        /// reading and writing session state.
        /// </summary>
        public IReadOnlyCollection<Type> KnownTypes => this.suspensionManager.KnownTypes;

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
        public INavigationService RegisterFrame(Frame frame, Type defaultPage, NavigationContextBase defaultNavigationContext = null, bool restoreState = true)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            if (defaultPage == null)
            {
                throw new ArgumentNullException(nameof(defaultPage));
            }

            if (this.GetNavigationService(frame) != null)
            {
                throw new InvalidOperationException("Frame already associated with a NavigationService.");
            }

            // Clean up references before adding a new one.
            this.RemoveFrame();

            NavigationService navigationService = new NavigationService(frame, new NavigationContextService());

            // Ensure navigationService is fully registered before restoring state
            // or navigating so that it can be accessed by PageBase.
            NavigationServices.Add(navigationService);

            FrameState state;
            if (restoreState && this.suspensionManager.TryGetState(navigationService.Name, out state))
            {
                // This triggers NavigatedTo on PageBase
                navigationService.RestoreState(state);
            }

            // If state wasn't restored, navigate to a default page to populate the Frame.
            if (navigationService.RootFrame.Content == null)
            {
                navigationService.Navigate(defaultPage, defaultNavigationContext);
            }

            return navigationService;
        }

        /// <summary>
        /// Unregisters a Frame and its NavigationService, removing all references to both the
        /// frame and navigation service. Unregisters the frame from the NavigationContextService.
        /// </summary>
        /// <param name="frame">The frame to unregister.</param>
        public void UnregisterFrame(Frame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            this.RemoveFrame(frame);
        }

        /// <summary>
        /// Retrieves the NavigationService for a registered Frame.
        /// </summary>
        /// <param name="frame">The Frame to get the NavigationService for.</param>
        /// <returns>The NavigationService for the provided frame.</returns>
        public INavigationService GetNavigationService(Frame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            return NavigationServices.FirstOrDefault(x => x.RootFrame == frame);
        }

        /// <summary>
        /// Saves the current navigation state in preparation for
        /// app suspension.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        public async Task SaveStateAsync()
        {
            // Clean up NavigationServices so we don't save the state of services with no Frame.
            this.RemoveFrame();

            // Save state to disk.
            await this.suspensionManager.SaveAsync(NavigationServices);
        }

        /// <summary>
        /// Restores the current navigation state from disk.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        public async Task RestoreStateAsync()
        {
            await this.suspensionManager.RestoreAsync();
        }

        /// <summary>
        /// Adds the provided types to a known type list so that they
        /// can be serialized when <see cref="SaveStateAsync" /> is called.
        /// </summary>
        /// <param name="types">The known types to add.</param>
        public void AddKnownTypes(List<Type> types)
        {
            if (types != null)
            {
                foreach (Type type in types)
                {
                    this.suspensionManager.AddKnownType(type);
                }
            }
        }

        /// <summary>
        /// Removes the NavigationService for the provided frame and cleans up any NavigationServices where their
        /// Frame has been garbage collected.
        /// </summary>
        /// <param name="frame">The frame to remove or null to just clean up invalid WeakReferences.</param>
        private void RemoveFrame(Frame frame = null)
        {
            IEnumerable<NavigationService> servicesToRemove = NavigationServices.Where(x => x.GetFrameSafe() == null || x.GetFrameSafe() == frame);
            foreach (NavigationService service in servicesToRemove.ToList())
            {
                this.suspensionManager.DeleteState(service.Name);
                NavigationServices.Remove(service);
            }
        }
    }
}
