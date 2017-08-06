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
        string baseParameters;
        List<string> actions = new List<string>();

        public BufferedActions(NameValueCollection baseParameters)
        {
            this.baseParameters = $"?rec=1&apiv=1&{baseParameters}&";
        }

        public void Add(NameValueCollection parameters)
        {
            actions.Add(baseParameters + parameters);
        }

        public override string ToString()
        {
            var data = new Dictionary<string, object>();
            data["requests"] = actions;
            return JsonConvert.SerializeObject(data);
        }


    }
}
