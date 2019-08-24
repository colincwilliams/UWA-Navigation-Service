// <copyright file="INavigatableViewModel.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.UwaNavigationService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for a ViewModel for a page that can be navigated to.
    /// </summary>
    public interface INavigatableViewModel
    {
        /// <summary>
        /// Activates the ViewModel with the given NavigationContext.
        /// </summary>
        /// <param name="navigationService">The navigation service for this view model.</param>
        /// <param name="navigationContext">The context in which the page is being activated.</param>
        /// /// <param name="pageState">The previously saved state of the page if it exists, otherwise null.</param>
        /// <returns>The task for this operation.</returns>
        Task Activate(INavigationService navigationService, NavigationContextBase navigationContext, IReadOnlyPageState pageState);

        /// <summary>
        /// Deactivates the view model, giving it a chance to save its state and delete any event subscriptions.
        /// </summary>
        /// <param name="pageState">A pageState to save to. This will always be provided.</param>
        void Deactivate(IDictionary<string, object> pageState);
    }
}
