using NUnit.Framework;
using System;
namespace Xamarin.Piwik.Tests
{
    [TestFixture()]
    public class TestAnalytics
    {
        [Test()]
        public async System.Threading.Tasks.Task TestTrackingPageVisit()
        {
            var analytics = new Analytics("https://tracktest.365farmnet.com", 3);
            analytics.TrackPage("Main");
            await analytics.Dispatch();
        }
    }
}
