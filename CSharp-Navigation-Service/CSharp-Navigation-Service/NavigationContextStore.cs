//-----------------------------------------------------------------------
// <summary>A storage location for navigation contexts.</summary>
// <copyright file="NavigationContextStore.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Collections.Generic;

    /// <summary>
    /// A storage location for navigation contexts.
    /// </summary>
    public class NavigationContextStore : Dictionary<long, NavigationContextBase>
    {
    }
}
