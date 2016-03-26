//-----------------------------------------------------------------------
// <summary>Class used to hold the event data required when a page attempts to save state.</summary>
// <copyright file="SaveStateEventArgs.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        public SaveStateEventArgs(Dictionary<string, object> pageState)
            : base()
        {
            this.PageState = pageState;
        }

        /// <summary>
        /// Gets an empty dictionary to be populated with serializable state.
        /// </summary>
        public Dictionary<string, object> PageState { get; private set; }
    }
}
