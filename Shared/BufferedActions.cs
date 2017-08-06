using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xamarin.Piwik
{
    public class BufferedActions
    {
        string baseUri;
        List<string> actions = new List<string>();

        public BufferedActions(string baseUri)
        {
            this.baseUri = baseUri;
        }

        public void Add(NameValueCollection action)
        {
            actions.Add(baseUri + action);
        }

        public override string ToString()
        {
            return string.Format("[BufferedActions]\n  " + String.Join("\n  ", actions));
        }
    }
}
