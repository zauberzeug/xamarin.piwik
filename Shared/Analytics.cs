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

        public Analytics(string apiUrl, int siteId)
        {
            var visitor = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16).ToUpper(); // TODO persistent visitor id
            this.apiUrl = $"{apiUrl}/piwik.php";
            baseParameters = CreateParameters();
            baseParameters["idsite"] = siteId.ToString();
            baseParameters["_id"] = visitor;
            actions = new ActionBuffer(baseParameters);
        }

        public void TrackPage(string name, string path = "/main")
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = $"http:/{path}";

            lock (actions)
                actions.Add(parameters);
        }

        public async Task Dispatch()
        {

            var actionsToDispatch = actions;
            lock (actionsToDispatch)
                actions = new ActionBuffer(baseParameters); // new action buffer to gather tracking infos while we dispatch

            Console.WriteLine(actionsToDispatch);
            var content = new StringContent(actionsToDispatch.ToString(), Encoding.UTF8, "application/json");
            try {
                var response = await httpClient.PostAsync(apiUrl, content);
                if (response.StatusCode == HttpStatusCode.OK) {
                    actionsToDispatch.Clear();
                    return;
                } else {
                    Console.WriteLine(response);
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            lock (actions)
                actions.Prepend(actionsToDispatch); // if dispatching was unsucessful, we need to keep the actions
        }

        NameValueCollection CreateParameters()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            // TODO add cdt as unix time stamp (https://github.com/piwik/piwik/issues/8998)
            return parameters;
        }
    }
}
