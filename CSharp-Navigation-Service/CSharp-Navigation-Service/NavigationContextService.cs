// <copyright file="NavigationContextService.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System;

    /// <summary>
    /// A service to manage NavigationContexts during navigation. Stores them
    /// with a related ID so that they may be retrieved later.
    /// </summary>
    internal class NavigationContextService : INavigationContextService
    {
        private long currentId;
        private NavigationContextStore contextStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContextService" /> class.
        /// </summary>
        public NavigationContextService()
        {
            this.contextStore = new NavigationContextStore();
        }

        /// <summary>
        /// Gets the identifier for the next context being stored.
        /// Is incremented with every access.
        /// </summary>
        protected long CurrentId
        {
            get { return this.currentId++; }
        }

        /// <summary>
        /// Adds a navigation context for storage.
        /// </summary>
        /// <param name="context">The context to be stored.</param>
        /// <returns>The ID of the context for later retrieval.</returns>
        public long Add(NavigationContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            long id = this.CurrentId;
            this.contextStore.Add(id, context);

            return id;
        }

        /// <summary>
        /// Gets a NavigationContext from the context store.
        /// </summary>
        /// <param name="id">The ID of the context to get.</param>
        /// <returns>The context to retrieve or null if the context could not be found.</returns>
        public NavigationContextBase Get(long id)
        {
            if (this.contextStore.ContainsKey(id))
            {
                return this.contextStore[id];
            }

            return null;
        }

        /// <summary>
        /// Gets a state object representing the current state of the context service.
        /// </summary>
        /// <returns>The state object.</returns>
        public NavigationContextServiceState SaveState()
        {
            return new NavigationContextServiceState()
            {
                CurrentId = this.currentId,
                Store = this.contextStore
            };
        }

        /// <summary>
        /// Restores the state of the context service to the provided state.
        /// </summary>
        /// <param name="state">The state to restore to.</param>
        public void RestoreState(NavigationContextServiceState state)
        {
            if (state != null)
            {
                this.currentId = state.CurrentId;
                this.contextStore = state.Store;
            }
        }
    }
}
