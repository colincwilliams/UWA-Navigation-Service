// <copyright file="MyNavigationContext.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SampleCommon
{
    using ColinCWilliams.UwaNavigationService;

    /*********************************************************
     * All of your NavigationContexts must inherit from
     * NavigationContextBase.
     *
     * While NavigationContextBase doesn't provide any
     * specific functionality, it forces awareness of how
     * these objects are being used.
     *
     * A NavigationContext should only store primitive
     * types, enums, or extremely compact classes as each
     * context could potentially be kept in memory for the
     * lifetime of the application.
     *
     * Notes:
     *   - This class must be serializble. The easiest way
     *     to get this is have a parameterless constructor.
     *   - After creating a new context, don't forget to
     *     update your known types in App.xaml.cs!
     *********************************************************/
    public class MyNavigationContext : NavigationContextBase
    {
        public string Value1 { get; set; }

        public string Value2 { get; set; }

        public string Value3 { get; set; }
    }
}
