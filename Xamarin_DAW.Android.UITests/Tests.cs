using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace Xamarin_DAW.Android.UITests
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android.StartApp();
        }

        [Test]
        public void AppDidLaunch()
        {
            app.WaitForElement(c => c.Id("content"));
            app.Screenshot("App launched");
        }
    }
}
