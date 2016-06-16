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
        /// Registers a <see cref="Frame"/> instance to allow its navigation history to be saved
        /// and restored.  Frames should be registered once
        /// immediately after creation if they will participate in session state management.  Upon
        /// registration if state has already been restored for the specified key
        /// the navigation history will immediately be restored.  Subsequent invocations of
        /// <see cref="RestoreAsync"/> will also restore navigation history.
        /// </summary>
        /// <param name="frame">An instance whose navigation history should be managed by
        /// <see cref="SuspensionManager"/></param>
        /// <param name="sessionStateKey">A unique key into used to
        /// store navigation-related information.</param>
        /// <param name="sessionBaseKey">An optional key that identifies the type of session.
        /// This can be used to distinguish between multiple application launch scenarios.</param>
        void RegisterFrame(Frame frame, string sessionStateKey, string sessionBaseKey = null);

        /// <summary>
        /// Disassociates a <see cref="Frame"/> previously registered.
        /// Any navigation state previously captured will be
        /// removed.
        /// </summary>
        /// <param name="frame">An instance whose navigation history should no longer be
        /// managed.</param>
        void UnregisterNavigationService(Frame frame);

        /// <summary>
        /// Restores previously saved session state. Any <see cref="Frame"/> instances
        /// registered with <see cref="RegisterFrame"/> will also restore their prior navigation
        /// state, which in turn gives their active <see cref="Page"/> an opportunity restore its
        /// state.
        /// </summary>
        /// <param name="sessionBaseKey">An optional key that identifies the type of session.
        /// This can be used to distinguish between multiple application launch scenarios.</param>
        /// <returns>An asynchronous task that reflects when session state has been read.  The
        /// content of SessionState should not be relied upon until this task
        /// completes.</returns>
        Task RestoreAsync(string sessionBaseKey = null);

        /// <summary>
        /// Save the current SessionState.  Any <see cref="Frame"/> instances
        /// registered with <see cref="RegisterFrame"/> will also preserve their current
        /// navigation stack, which in turn gives their active <see cref="Page"/> an opportunity
        /// to save its state.
        /// </summary>
        /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
        Task SaveAsync();

        /// <summary>
        /// Provides storage for session state associated with the specified <see cref="Frame"/>.
        /// Frames that have been previously registered with <see cref="RegisterFrame"/> have
        /// their session state saved and restored automatically as a part of the global
        /// SessionState.  Frames that are not registered have transient state
        /// that can still be useful when restoring pages that have been discarded from the
        /// navigation cache.
        /// </summary>
        /// <param name="frame">The instance for which session state is desired.</param>
        /// <returns>A collection of state subject to the same serialization mechanism as
        /// SessionState.</returns>
        Dictionary<string, object> SessionStateForFrame(Frame frame);
    }
}
