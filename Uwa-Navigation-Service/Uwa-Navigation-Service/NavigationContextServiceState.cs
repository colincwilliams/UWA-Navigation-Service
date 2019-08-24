// <copyright file="NavigationContextServiceState.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The state of a NavigationContextService for saving or restoring.
    /// </summary>
    [DataContract]
    internal class NavigationContextServiceState
    {
        /// <summary>
        /// Gets or sets the ID that should be used for the next item being stored.
        /// </summary>
        [DataMember]
        public long CurrentId { get; set; }

        /// <summary>
        /// Gets or sets the navigation contexts being stored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Needs to be serialized")]
        [DataMember]
        public NavigationContextStore Store { get; set; }
    }
}
