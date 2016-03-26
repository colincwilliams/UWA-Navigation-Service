//-----------------------------------------------------------------------
// <summary>The state of a NavigationContextService for saving or restoring.</summary>
// <copyright file="NavigationContextServiceState.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The state of a NavigationContextService for saving or restoring.
    /// </summary>
    [DataContract]
    public class NavigationContextServiceState
    {
        /// <summary>
        /// Gets or sets the ID that should be used for the next item being stored.
        /// </summary>
        [DataMember]
        public long CurrentId { get; set; }

        /// <summary>
        /// Gets or sets the navigation contexts being stored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Needs to be serialized"), DataMember]
        public NavigationContextStore Store { get; set; }
    }
}
