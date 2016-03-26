//-----------------------------------------------------------------------
// <summary>Defines the interface for a ViewModel for a page that can be navigated to.</summary>
// <copyright file="INavigatableViewModel.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for a ViewModel for a page that can be navigated to.
    /// </summary>
    public interface INavigatableViewModel
    {
        /// <summary>
        /// Activates the ViewModel with the given NavigationContext.
        /// </summary>
        /// <param name="navigationContext">The context in which the page is being activated.</param>
        /// <returns>The task for this operation.</returns>
        Task Activate(NavigationContextBase navigationContext);
    }
}
