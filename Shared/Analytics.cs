using System;
using System.Web;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net;

namespace Xamarin.Piwik
{
    public class Analytics
    {
        string apiUrl;
        ActionBuffer actions;
        NameValueCollection baseParameters;
        HttpClient httpClient = new HttpClient();
        Random random = new Random();

        public Analytics(string apiUrl, int siteId)
        {
            var visitor = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16).ToUpper(); // TODO persistent visitor id
            this.apiUrl = $"{apiUrl}/piwik.php";
            baseParameters = HttpUtility.ParseQueryString(string.Empty);
            baseParameters["idsite"] = siteId.ToString();
            baseParameters["_id"] = visitor;
            baseParameters["cid"] = visitor;
            actions = new ActionBuffer(baseParameters);

            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public bool Verbose { get; set; } = false;

        public int UnsentActions { get { lock (actions) return actions.Count; } }

        /// <summary>
        /// Tracks a page visit.
        /// </summary>
        /// <param name="name">page name (eg. "Settings" or "Settings / I18N / Units"</param>
        public void TrackPage(string name)
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = $"http://app-has-no-url";

            lock (actions)
                actions.Add(parameters);
        }

        public async Task Dispatch()
        {
            var actionsToDispatch = actions;
            lock (actionsToDispatch)
                actions = new ActionBuffer(baseParameters); // new action buffer to gather tracking infos while we dispatch

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


    }
}
