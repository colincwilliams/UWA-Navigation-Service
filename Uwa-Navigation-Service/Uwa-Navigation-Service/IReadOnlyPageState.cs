// <copyright file="IReadOnlyPageState.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.UwaNavigationService
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents readonly PageState, which stores state information for a specific page.
    /// </summary>
    public interface IReadOnlyPageState : IReadOnlyDictionary<string, object>
    {
    }
}
