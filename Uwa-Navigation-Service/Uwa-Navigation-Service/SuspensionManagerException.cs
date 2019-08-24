// <copyright file="SuspensionManagerException.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.UwaNavigationService
{
    using System;

    /// <summary>
    /// An exception that is thrown from within SuspensionManager.
    /// </summary>
    public class SuspensionManagerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionManagerException" /> class.
        /// </summary>
        public SuspensionManagerException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionManagerException" /> class.
        /// </summary>
        /// <param name="message">The message for the Exception.</param>
        public SuspensionManagerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionManagerException" /> class.
        /// </summary>
        /// <param name="e">The Exception to use as the inner exception with a default message.</param>
        public SuspensionManagerException(Exception e)
            : base("SuspensionManager failed", e)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionManagerException" /> class.
        /// </summary>
        /// <param name="message">The message to use for the Exception.</param>
        /// <param name="e">The exception to use as the inner exception.</param>
        public SuspensionManagerException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
