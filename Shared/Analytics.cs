using System;
using System.Web;
using System.Collections.Specialized;


namespace Xamarin.Piwik
{
    public class Analytics
    {
        BufferedActions actions;

        public Analytics(string apiUrl, int siteId)
        {
            var visitor = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16).ToUpper(); // TODO persistent visitor id
            var baseUri = $"{apiUrl}/piwik.php?rec=1&_id={visitor}&apiv=1&";
            var baseParameters = CreateParameters();
            baseParameters["idsite"] = siteId.ToString();
            baseParameters["_id"] = visitor;
            actions = new BufferedActions(baseUri, baseParameters);
        }

        public void TrackPage(string name, string path = "/")
        {
            var parameters = CreateParameters();
            parameters["action_name"] = name;
            parameters["url"] = path;

            actions.Add(parameters);
        }

        public void Dispatch()
        {
            Console.WriteLine(actions);
        }

        NameValueCollection CreateParameters()
        {
            return HttpUtility.ParseQueryString(string.Empty);
        }
    }
}
