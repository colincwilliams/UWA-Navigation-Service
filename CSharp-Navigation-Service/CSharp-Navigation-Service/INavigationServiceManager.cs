// <copyright file="INavigationServiceManager.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Manages all registered NavigationServices and provides support
    /// for suspending and restoring them.
    /// </summary>
    internal interface INavigationServiceManager
    {
        /// <summary>
        /// Gets a readonly Collection of custom types provided to the <see cref="DataContractSerializer"/> when
        /// reading and writing session state.
        /// </summary>
        IReadOnlyCollection<Type> KnownTypes { get; }

        /// <summary>
        /// Adds the provided types to a known type list so that they
        /// can be serialized when <see cref="SaveStateAsync" /> is called.
        /// </summary>
        /// <param name="types">The known types to add.</param>
        void AddKnownTypes(List<Type> types);

        /// <summary>
        /// Retrieves the NavigationService for a registered Frame.
        /// </summary>
        /// <param name="frame">The Frame to get the NavigationService for.</param>
        /// <returns>The NavigationService for the provided frame.</returns>
        INavigationService GetNavigationService(Frame frame);

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
        INavigationService RegisterFrame(Frame frame, Type defaultPage, NavigationContextBase defaultNavigationContext = null, bool restoreState = true);

        /// <summary>
        /// Restores the current navigation state from disk.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        Task RestoreStateAsync();

        /// <summary>
        /// Saves the current navigation state in preparation for
        /// app suspension.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        Task SaveStateAsync();

        /// <summary>
        /// Unregisters a Frame and its NavigationService, removing all references to both the
        /// frame and navigation service. Unregisters the frame from the NavigationContextService.
        /// </summary>
        /// <param name="frame">The frame to unregister.</param>
        void UnregisterFrame(Frame frame);
    }
}