using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace Xamarin.Piwik.Tests
{
    [TestFixture()]
    public class TestAnalytics
    {
        [Test()]
        public async Task TestTrackingPageVisits()
        {
            var url = GetLocalhostAddress();
            var analytics = new Analytics(url, 3);
            analytics.Verbose = true;

            analytics.TrackPage("Main");
            analytics.TrackPage("LevelA / Sub");

            using (var receivedData = BasicHttpServer(url)) {
                await analytics.Dispatch();
                var json = JObject.Parse(await receivedData);
                var main = json["requests"][0].ToString();
                Assert.That(main, Does.Contain("action_name=Main"));
                var sub = json["requests"][1].ToString();
                Assert.That(sub, Does.Contain("action_name=LevelA+%2f+Sub"));
            }
        }

        Task<string> BasicHttpServer(string url)
        {
            return Task.Run(() => {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();

                // GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                var body = new StreamReader(context.Request.InputStream).ReadToEnd();

                HttpListenerResponse response = context.Response;
                Stream stream = response.OutputStream;
                var writer = new StreamWriter(stream);
                writer.Write("");
                writer.Close();

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
