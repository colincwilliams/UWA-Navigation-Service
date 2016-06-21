// <copyright file="ISuspensionManager.cs" company="Colin C. Williams">
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
    /// An interface for managing app suspension and restoration.
    /// </summary>
    public interface ISuspensionManager
    {
        /// <summary>
        /// Gets a List of custom types provided to the <see cref="DataContractSerializer"/> when
        /// reading and writing session state.
        /// </summary>
        IReadOnlyCollection<Type> KnownTypes { get; }

        /// <summary>
        /// Adds a type to the known types list for serialization.
        /// </summary>
        /// <param name="type">The type to add.</param>
        void AddKnownType(Type type);

        /// <summary>
        /// Save the current SessionState for the provided navigationServices. All NavigationServices
        /// will also preserve their current navigation stack, which in turn gives their active
        /// <see cref="Page"/> an opportunity to save its state.
        /// </summary>
        /// <param name="navigationServices">The Navigation Services to save state for.</param>
        /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
        Task SaveAsync(IEnumerable<NavigationService> navigationServices);

        /// <summary>
        /// Restores previously saved session state.
        /// </summary>
        /// <returns>An asynchronous task that reflects when session state has been read.  The
        /// content of SessionState should not be relied upon until this task
        /// completes.</returns>
        Task RestoreAsync();

        /// <summary>
        /// Restores the state of a NavigationService.
        /// </summary>
        /// <param name="name">The name of the NavigationService to get state for.</param>
        /// <returns>The state of the NavigationService if it was previously restored, otherwise null.</returns>
        FrameState GetState(string name);

        /// <summary>
        /// Deletes the restored session state for the NavigationService with the given name.
        /// </summary>
        /// <param name="name">Name of the NavigationService to delete state for.</param>
        void DeleteState(string name);
    }
}
