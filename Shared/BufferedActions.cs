using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xamarin.Piwik
{
    public class BufferedActions
    {
        string baseUri;
        List<string> actions = new List<string>();

        public BufferedActions(string baseUri, NameValueCollection baseParameters)
        {
            this.baseUri = baseUri + baseParameters + "&";
        }

        public void Add(NameValueCollection parameters)
        {
            actions.Add(baseUri + parameters);
        }

        public override string ToString()
        {
            return actions.First();

            var data = new Dictionary<string, object>();
            data["requests"] = actions;
            return JsonConvert.SerializeObject(data);
        }


    }
}
