using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PerpetualEngine.Storage;

namespace Xamarin.Piwik
{
    public class ActionBuffer
    {
        string baseParameters;
        List<string> actions = new List<string>();
        List<string> outbox = new List<string>();
        SimpleStorage storage;

        public ActionBuffer(NameValueCollection baseParameters, SimpleStorage storage)
        {
            this.baseParameters = $"?rec=1&apiv=1&{baseParameters}&";
            this.storage = storage;
        }

        public int Count { get { return actions.Count + outbox.Count; } }

        public void Add(NameValueCollection parameters)
        {
            actions.Add(baseParameters + parameters);
        }

        public void Prepend(ActionBuffer otherActions)
        {
            actions.InsertRange(0, otherActions.actions);
        }

        public void ClearOutbox()
        {
            lock (outbox) outbox.Clear();
        }

        public string CreateOutbox()
        {
            lock (outbox) lock (actions) {
                    outbox.AddRange(actions);
                    actions.Clear();
                }

            var data = new Dictionary<string, object>();
            data["requests"] = outbox;
            return JsonConvert.SerializeObject(data);
        }
    }
}
