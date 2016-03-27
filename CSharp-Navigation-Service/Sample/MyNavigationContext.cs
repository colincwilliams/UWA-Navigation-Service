using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
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
