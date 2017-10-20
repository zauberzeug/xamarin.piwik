using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Xamarin.Piwik.Tests
{
    [TestFixture()]
    public class TestAnalytics
    {
        Analytics analytics;
        string url;

        [SetUp]
        public void Setup()
        {
            url = GetLocalhostAddress();
            analytics = new Analytics(url, 3);
            analytics.Verbose = true;
            analytics.OptOut = false;
            Assert.That(analytics.UnsentActions, Is.EqualTo(0));
        }

        [TearDown]
        public void TearDown()
        {
            analytics.OptOut = true; // resetting persistence
            Assert.That(analytics.UnsentActions, Is.EqualTo(0));
        }

        [Test()]
        public async Task TestTrackingPageVisits()
        {
            analytics.TrackPage("Main");
            analytics.TrackPage("LevelA / Sub");

            var receivedData = MockedPiwikServer(url);
            await analytics.Dispatch();
            Assert.That(analytics.UnsentActions, Is.EqualTo(0));

            var json = JObject.Parse(await receivedData);
            var main = json["requests"][0].ToString();
            Assert.That(main, Does.Contain("action_name=Main"));
            var sub = json["requests"][1].ToString();
            Assert.That(sub, Does.Contain("action_name=LevelA+%2f+Sub"));
        }

        [Test()]
        public async Task TestTrackEvent()
        {
            analytics.TrackEvent("cat", "some action");

            var receivedData = MockedPiwikServer(url);
            await analytics.Dispatch();
            Assert.That(analytics.UnsentActions, Is.EqualTo(0));

            var json = JObject.Parse(await receivedData);
            var main = json["requests"][0].ToString();
            Assert.That(main, Does.Not.Contain("action_name="), "events do not have an action name (checked with javascript and android sdk)");
            Assert.That(main, Does.Contain("e_a=some+action"));
            Assert.That(main, Does.Contain("e_c=cat"));
            Assert.That(main, Does.Contain("url="), "events should always send an url because piwik can be configured to drop data which are not targeted at a specific domain");
        }


        [Test()]
        public async Task TestServerErrorWhileDispatching()
        {
            analytics.TrackPage("Main");

            var receivedData = MockedPiwikServer(url, statusCode: 500);
            await analytics.Dispatch();
            await receivedData;
            Assert.That(analytics.UnsentActions, Is.EqualTo(1));

            receivedData = MockedPiwikServer(url);
            await analytics.Dispatch();
            await receivedData;
            Assert.That(analytics.UnsentActions, Is.EqualTo(0));
        }

        [Test()]
        public async Task TestConnectionErrorWhileDispatching()
        {
            analytics.TrackPage("Main");

            await analytics.Dispatch();
            Assert.That(analytics.UnsentActions, Is.EqualTo(1));

            var receivedData = MockedPiwikServer(url);
            Thread.Sleep(100); // delay before dispaching to make sure the mocked server has opend the port
            await analytics.Dispatch();
            await receivedData;
            Assert.That(analytics.UnsentActions, Is.EqualTo(0));

        }

        [Test()]
        public void TestOptOut()
        {
            analytics.TrackPage("Main");
            Assert.That(analytics.UnsentActions, Is.EqualTo(1));

            analytics.OptOut = true;
            analytics.TrackPage("Main2");

            Assert.That(analytics.UnsentActions, Is.EqualTo(0));
        }

        Task<string> MockedPiwikServer(string url, int statusCode = 200)
        {
            return Task.Run(() => {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();

                // GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();

                var body = new StreamReader(context.Request.InputStream).ReadToEnd();

                HttpListenerResponse response = context.Response;

                try {
                    response.StatusCode = statusCode;
                    Stream stream = response.OutputStream;
                    using (var writer = new StreamWriter(stream))
                        writer.Write("");

                    listener.Stop();

                } catch (Exception e) {
                    Console.WriteLine(e);
                }

                Assert.That(context.Request.ContentType, Is.EqualTo("application/json; charset=utf-8"));
                Assert.That(context.Request.HttpMethod, Is.EqualTo("POST"));

                return body;
            });
        }

        string GetLocalhostAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return $"http://localhost:{port}/";
        }
    }
}
