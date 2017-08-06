using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xamarin.Piwik
{
    public class ActionBuffer
    {
        string baseParameters;
        List<string> actions = new List<string>();

        public ActionBuffer(NameValueCollection baseParameters)
        {
            this.baseParameters = $"?rec=1&apiv=1&{baseParameters}&";
        }

        public void Add(NameValueCollection parameters)
        {
            actions.Add(baseParameters + parameters);
        }

        public void Prepend(ActionBuffer otherActions)
        {
            actions.InsertRange(0, otherActions.actions);
        }

        public void Clear()
        {
            actions.Clear();
        }

        public override string ToString()
        {
            var data = new Dictionary<string, object>();
            data["requests"] = actions;
            return JsonConvert.SerializeObject(data);
        }
    }
}
