//-----------------------------------------------------------------------
// <summary>Base class for all Navigation Contexts.</summary>
// <copyright file="NavigationContextBase.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class for all Navigation Contexts. A navigation context stores information
    /// that should be passed to a page being navigated to.
    /// </summary>
    [DataContract]
    public abstract class NavigationContextBase
    {
    }
}
