using System;
using System.Web;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using PerpetualEngine.Storage;
using System.Timers;

namespace Xamarin.Piwik
{
    public class Analytics
    {
        string apiUrl;
        ActionBuffer actions;
        NameValueCollection baseParameters;
        NameValueCollection pageParameters;

        HttpClient httpClient = new HttpClient();
        Random random = new Random();
        SimpleStorage storage = SimpleStorage.EditGroup("xamarin.piwik");

        Timer timer = new Timer();

        public Analytics(string apiUrl, int siteId)
        {
            var visitor = GenerateId(16);
            if (storage.HasKey("visitor_id"))
                visitor = storage.Get("visitor_id");
            else {
                storage.Put("visitor_id", visitor);
            }

            this.apiUrl = apiUrl.StartsWith("https://requestb.in") ? apiUrl : $"{apiUrl}/piwik.php"; // requestb.in can be used for testing and debugging
            baseParameters = HttpUtility.ParseQueryString(string.Empty);
            baseParameters["idsite"] = siteId.ToString();
            baseParameters["_id"] = visitor;
            baseParameters["cid"] = visitor;

            pageParameters = HttpUtility.ParseQueryString(string.Empty);

            actions = new ActionBuffer(baseParameters);

            httpClient.Timeout = TimeSpan.FromSeconds(30);

            timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
            timer.Elapsed += async (s, args) => await Dispatch();
            timer.Start();
        }

        public bool Verbose { get; set; } = false;

        public int UnsentActions { get { lock (actions) return actions.Count; } }

        /// <summary>
        /// The base url used by the app (piwi's url parameter). Default is http://app
        /// </summary>
        public string AppUrl { get; set; } = "http://app";

        /// <summary>
        /// Tracks a page visit.
        /// </summary>
        /// <param name="name">page name (eg. "Settings", "Users", etc)</param>
        /// <param name="path">path which led to the page (eg. "/settings/language"), default is "/"</param>
        public void TrackPage(string name, string path = "/")
        {
            pageParameters["pv_id"] = GenerateId(6);
            pageParameters["url"] = $"{AppUrl}{path}";

            var parameters = CreateParameters();
            parameters["action_name"] = name;

            parameters.Add(pageParameters);

            lock (actions)
                actions.Add(parameters);
        }

        /// <summary>
        /// Tracks an page related event.
        /// </summary>
        /// <param name="name">event name (eg. "play", "refresh", etc)</param>
        public void TrackPageEvent(string name)
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = $"{AppUrl}";

            parameters.Add(pageParameters);

            lock (actions)
                actions.Add(parameters);
        }

        /// <summary>
        /// Tracks an non-page related event.
        /// </summary>
        /// <param name="name">event name (eg. "Auto-Update", "DB cleanup", etc)</param>
        public void TrackEvent(string name)
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = $"{AppUrl}";

            lock (actions)
                actions.Add(parameters);
        }

        public void LeavingTheApp()
        {
            TrackPage("Close");
            Dispatch();
        }

        public async Task Dispatch() // TODO run in background: http://arteksoftware.com/backgrounding-with-xamarin-forms/
        {
            var actionsToDispatch = actions;
            lock (actionsToDispatch) {
                if (actionsToDispatch.Count == 0)
                    return;
                actions = new ActionBuffer(baseParameters); // new action buffer to gather tracking infos while we dispatch
            }

            Log(actionsToDispatch);
            var content = new StringContent(actionsToDispatch.ToString(), Encoding.UTF8, "application/json");

            try {
                var response = await httpClient.PostAsync(apiUrl, content);
                if (response.StatusCode == HttpStatusCode.OK)
                    return;

                Log(response);
            } catch (Exception e) {
                Log(e);
                httpClient.CancelPendingRequests();
            }

            lock (actions)
                actions.Prepend(actionsToDispatch); // if dispatching failed, we need to keep the actions
        }

        NameValueCollection CreateParameters()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["rand"] = random.Next().ToString();
            parameters["cdt"] = (DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString(); // TODO dispatching cdt older thant 24 h needs token_auth in bulk request
            return parameters;
        }

        void Log(object msg)
        {
            if (Verbose)
                Console.WriteLine(msg.ToString());
        }

        private static string GenerateId(int length)
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, length).ToUpper();
        }
    }
}