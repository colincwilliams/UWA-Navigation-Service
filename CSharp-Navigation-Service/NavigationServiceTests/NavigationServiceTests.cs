using ColinCWilliams.CSharpNavigationService;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavigationServiceTests
{
    [TestClass]
    public class NavigationServiceTests
    {
        private NavigationService navigationService;

        private Frame Frame { get; set; }

        private INavigationService INavigationService
        {
            get { return this.navigationService; }
            set { this.navigationService = value as NavigationService; }
        }

        private NavigationService NavigationService
        {
            get { return this.navigationService; }
            set { this.navigationService = value; }
        }

        [TestMethod]
        public async Task Navigate()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    this.Setup();

                    this.TestNavigate(null);
                    this.TestNavigate(new TestNavigationContext());

                    this.Cleanup();
                });
        }

        private void Setup(NavigationContextBase context = null)
        {
            this.Frame = new Frame()
            {
                Name = "TestFrame"
            };

            this.INavigationService = NavigationService.RegisterFrame(this.Frame, typeof(TestPage), context);

            Assert.AreEqual(this.Frame.Name, this.NavigationService.Name);
            Assert.IsTrue(this.Frame.Content is TestPage);
            Assert.IsFalse(this.INavigationService.CanGoBack);
            Assert.IsFalse(this.INavigationService.CanGoForward);

            this.ValidateViewModelState(TestViewModel.ViewModels.Last(), context);
        }

        private void Cleanup()
        {
            NavigationService.UnregisterFrame(this.Frame);
            TestViewModel.ResetAll();
        }

        private void TestNavigate(NavigationContextBase context = null)
        {
            int backstackBeforeNavigate = this.Frame.BackStackDepth;
            TestViewModel vmBeforeNavigate = TestViewModel.ViewModels.Last();

            this.INavigationService.Navigate(typeof(TestPage), context);

            Assert.IsTrue(vmBeforeNavigate.DeactivateCalled);
            Assert.AreEqual(0, vmBeforeNavigate.DeactivatePageState.Count);
            Assert.AreEqual(backstackBeforeNavigate + 1, this.Frame.BackStackDepth);
            this.ValidateViewModelState(TestViewModel.ViewModels.Last(), context);
        }

        private void ValidateViewModelState(TestViewModel vm, NavigationContextBase expectedContext)
        {
            Assert.IsTrue(vm.ActivateCalled);
            Assert.IsFalse(vm.DeactivateCalled);
            Assert.AreSame(this.INavigationService, vm.NavigationService);
            Assert.AreSame(expectedContext, vm.NavigationContext);
            Assert.IsNull(vm.ActivatePageState);
        }
    }
}
