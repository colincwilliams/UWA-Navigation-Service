using ColinCWilliams.CSharpNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigationServiceTests
{
    public class TestViewModel : INavigatableViewModel
    {
        public static readonly List<TestViewModel> ViewModels = new List<TestViewModel>();

        public TestViewModel()
        {
            ViewModels.Add(this);
        }

        public bool ActivateCalled { get; private set; }

        public bool DeactivateCalled { get; private set; }

        public INavigationService NavigationService { get; private set; }

        public NavigationContextBase NavigationContext { get; private set; }

        public IReadOnlyDictionary<string, object> ActivatePageState { get; private set; }

        public IDictionary<string, object> DeactivatePageState { get; private set; }

        public static void ResetAll()
        {
            ViewModels.Clear();
        }

        public Task Activate(INavigationService navigationService, NavigationContextBase navigationContext, IReadOnlyDictionary<string, object> pageState)
        {
            this.Reset();

            this.ActivateCalled = true;
            this.NavigationService = navigationService;
            this.NavigationContext = navigationContext;
            this.ActivatePageState = pageState;

            return Task.FromResult(false);
        }

        public void Deactivate(IDictionary<string, object> pageState)
        {
            this.DeactivateCalled = true;
            this.DeactivatePageState = pageState;
        }

        public void Reset()
        {
            this.ActivateCalled = false;
            this.DeactivateCalled = false;

            this.NavigationService = null;
            this.NavigationContext = null;
            this.ActivatePageState = null;
            this.DeactivatePageState = null;
        }
    }
}
