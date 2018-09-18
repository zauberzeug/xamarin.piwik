using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Piwik.Tests
{
    public class PiwikMocker
    {
        public static Task<string> Receive(string url, CancellationToken token = new CancellationToken(), int statusCode = 200)
        {
            return Task.Run(() => {
                using (token.Register(Thread.CurrentThread.Abort)) {
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
                }
            }, token);
        }

        public static async Task<string> SubmitAndReceive(PiwikAnalytics analytics, string url)
        {
            var tokenSource = new CancellationTokenSource();

            var receivedData = PiwikMocker.Receive(url, tokenSource.Token);
            var dispatched = await analytics.Dispatch();
            if (dispatched)
                return await receivedData;

            tokenSource.Cancel();
            return "";
        }
    }
}
