// <copyright file="INavigationContextService.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    /// <summary>
    /// A service to manage NavigationContexts during navigation. Stores them
    /// with a related ID so that they may be retrieved later.
    /// </summary>
    public interface INavigationContextService
    {
        /// <summary>
        ///  Adds a context to the context store.
        /// </summary>
        /// <param name="context">The context to store.</param>
        /// <returns>The ID of the context for later retrieval.</returns>
        long Add(NavigationContextBase context);

        /// <summary>
        /// Retrieves a context from the store with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the context to retrieve.</param>
        /// <returns>The context if it exists in the store or null if it doesn't.</returns>
        NavigationContextBase Get(long id);

        /// <summary>
        /// Gets a state object representing the current state of the context service.
        /// </summary>
        /// <returns>The state object.</returns>
        NavigationContextServiceState SaveState();

        /// <summary>
        /// Restores the state of the context service to the provided state.
        /// </summary>
        /// <param name="state">The state to restore to.</param>
        void RestoreState(NavigationContextServiceState state);
    }
}
