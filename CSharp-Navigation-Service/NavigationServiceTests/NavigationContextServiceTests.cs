namespace NavigationServiceTests
{
    using System;
    using ColinCWilliams.CSharpNavigationService;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    [TestClass]
    public class NavigationContextServiceTests
    {
        [TestMethod]
        public void AddNotNullItem()
        {
            NavigationContextService service = new NavigationContextService();
            TestNavigationContext testContext1 = new TestNavigationContext();
            TestNavigationContext testContext2 = new TestNavigationContext();

            long id1 = service.Add(testContext1);
            long id2 = service.Add(testContext2);
            long id1a = service.Add(testContext1);

            Assert.AreNotEqual(id1, id2, "Ids from two different adds should be unique.");
            Assert.AreNotEqual(id1, id1a, "Ids from two different adds, even if they are the same context, should be unique.");

            Assert.AreSame(testContext1, service.Get(id1));
            Assert.AreSame(testContext2, service.Get(id2));
            Assert.AreSame(testContext1, service.Get(id1a));
        }

        [TestMethod]
        public void AddNullItem()
        {
            NavigationContextService service = new NavigationContextService();
            bool exceptionThrown = false;

            try
            {
                service.Add(null);
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected ArgumentNullException when trying to add null to context service.");
        }

        [TestMethod]
        public void GetItemNotExists()
        {
            NavigationContextService service = new NavigationContextService();
            Assert.IsNull(service.Get(123));
        }

        [TestMethod]
        public void SaveRestoreState()
        {
            NavigationContextService service = new NavigationContextService();
            TestNavigationContext testContext1 = new TestNavigationContext();
            TestNavigationContext testContext2 = new TestNavigationContext();
            TestNavigationContext testContext3 = new TestNavigationContext();

            long id1 = service.Add(testContext1);
            long id2 = service.Add(testContext2);

            NavigationContextServiceState state = service.SaveState();

            Assert.AreEqual(2, state.CurrentId);
            Assert.AreSame(testContext1, state.Store[id1]);
            Assert.AreSame(testContext2, state.Store[id2]);

            service = new NavigationContextService();
            service.RestoreState(state);

            long id3 = service.Add(testContext3);

            Assert.AreEqual(2, id3);
            Assert.AreSame(testContext1, service.Get(id1));
            Assert.AreSame(testContext2, service.Get(id2));
        }
    }
}
