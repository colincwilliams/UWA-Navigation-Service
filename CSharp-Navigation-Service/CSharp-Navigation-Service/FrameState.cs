// <copyright file="FrameState.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the saved state for a frame.
    /// </summary>
    public class FrameState
    {
        /// <summary>
        /// Gets or sets the Navigation state for a frame.
        /// </summary>
        public string Navigation { get; set; }

        /// <summary>
        /// Gets or sets the Navigation Context Service state for a frame.
        /// </summary>
        public NavigationContextServiceState ContextService { get; set; }

        /// <summary>
        /// Gets or sets the states for the individual pages this Navigation Service has seen.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for serialization.")]
        public IDictionary<string, PageState> PageStates { get; set; }
    }
}
