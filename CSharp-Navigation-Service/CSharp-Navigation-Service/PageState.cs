// <copyright file="PageState.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.CSharpNavigationService
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Stores state information for a specific page.
    /// </summary>
    public class PageState : Dictionary<string, object>
    {
    }
}
