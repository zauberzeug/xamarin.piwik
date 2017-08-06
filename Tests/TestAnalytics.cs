using NUnit.Framework;
using System;
namespace Xamarin.Piwik.Tests
{
    [TestFixture()]
    public class TestAnalytics
    {
        [Test()]
        public void TestTrackingPageVisit()
        {
            var analytics = new Analytics("https://tracktest.365farmnet.com", 13);
            analytics.TrackPage("Main");
            analytics.Dispatch();
        }
    }
}
