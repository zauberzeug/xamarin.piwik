using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xamarin.Piwik
{
    public class BufferedActions
    {
        string baseUri;
        List<string> actions = new List<string>();

        public BufferedActions(string baseUri, NameValueCollection baseParameters)
        {
            this.baseUri = baseUri + baseParameters;
        }

        public void Add(NameValueCollection parameters)
        {
            actions.Add(baseUri + parameters);
        }

        public override string ToString()
        {
            return string.Format("[BufferedActions]\n  " + String.Join("\n  ", actions));
        }
    }
}
