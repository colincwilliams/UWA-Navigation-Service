// <copyright file="INavigationService.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The interface for a NavigationService that supports passing complex types.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets the context service associated with this NavigationService.
        /// </summary>
        INavigationContextService ContextService { get; }

        /// <summary>
        /// Gets the states for the individual pages this Navigation Service has seen.
        /// </summary>
        IDictionary<string, PageState> PageStates { get; }

        /// <summary>
        /// Gets a value indicating whether calling <see cref="GoBack" /> will success.
        /// </summary>
        /// <returns>True if back navigation can occur, false otherwise.</returns>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a value indicating whether calling <see cref="GoForward" /> will success.
        /// </summary>
        /// <returns>True if forward navigation can occur, false otherwise.</returns>
        bool CanGoForward { get; }

        /// <summary>
        /// Navigate back a page in the back stack.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigate forward a page in the forward stack.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Navigate to the specified page with the provided context.
        /// </summary>
        /// <param name="pageType">The type of page to navigate to.</param>
        /// <param name="context">The context that should be provided to the page after navigation.</param>
        void Navigate(Type pageType, NavigationContextBase context = null);
    }
}
