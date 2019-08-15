// <copyright file="NavigationContextBase.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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
