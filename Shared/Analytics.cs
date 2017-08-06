using System;
using System.Web;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace Xamarin.Piwik
{
    public class Analytics
    {
        string apiUrl;
        BufferedActions actions;
        HttpClient httpClient = new HttpClient();

        public Analytics(string apiUrl, int siteId)
        {
            var visitor = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16).ToUpper(); // TODO persistent visitor id
            this.apiUrl = $"{apiUrl}/piwik.php";
            var baseParameters = CreateParameters();
            baseParameters["idsite"] = siteId.ToString();
            baseParameters["_id"] = visitor;
            actions = new BufferedActions(baseParameters);
        }

        public void TrackPage(string name, string path = "/main")
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = $"http:/{path}";

            actions.Add(parameters);
        }

        public async Task Dispatch()
        {
            Console.WriteLine(actions);
            var content = new StringContent(actions.ToString(), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);
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
