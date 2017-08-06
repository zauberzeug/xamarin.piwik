using System;
using System.Web;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xamarin.Piwik
{
    public class Analytics
    {
        BufferedActions actions;
        HttpClient httpClient = new HttpClient();

        public Analytics(string apiUrl, int siteId)
        {
            var visitor = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16).ToUpper(); // TODO persistent visitor id
            var baseUri = $"{apiUrl}/piwik.php?rec=1&&apiv=1&";
            var baseParameters = CreateParameters();
            baseParameters["idsite"] = siteId.ToString();
            baseParameters["_id"] = visitor;
            actions = new BufferedActions(baseUri, baseParameters);
        }

        public void TrackPage(string name, string path = "/")
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = $"http://root{path}";

            actions.Add(parameters);
        }

        public async Task Dispatch()
        {
            Console.WriteLine(actions);
            var response = await httpClient.GetAsync(actions.ToString());
            Console.WriteLine(response);
        }

        NameValueCollection CreateParameters()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            // TODO add cdt as unix time stamp (https://github.com/piwik/piwik/issues/8998)
            return parameters;
        }
    }
}
