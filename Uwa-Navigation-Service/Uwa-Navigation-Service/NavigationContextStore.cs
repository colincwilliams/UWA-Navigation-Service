// <copyright file="NavigationContextStore.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.UwaNavigationService
{
    using System.Collections.Generic;

    /// <summary>
    /// A storage location for navigation contexts.
    /// </summary>
    internal class NavigationContextStore : Dictionary<long, NavigationContextBase>
    {
    }
}
