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
            var baseUri = $"{apiUrl}/piwik.php?idsite={siteId}&rec=1&_id={visitor}&apiv=1&";
            actions = new BufferedActions(baseUri);
        }

        public void TrackPage(string name, string path = "/")
        {
            var action = CreateAction();
            action["action_name"] = name;
            action["url"] = path;

            actions.Add(action);
        }

        public void Dispatch()
        {
            Console.WriteLine(actions);
        }

        NameValueCollection CreateAction()
        {
            return HttpUtility.ParseQueryString(string.Empty);
        }
    }
}
