using System;
using Java.Lang;
using PerpetualEngine.Storage;
namespace Xamarin.Piwik
{
    public partial class PiwikAnalytics
    {
        string userAgent;
        string UserAgent {
            get {
                if (userAgent == null) {
                    userAgent = JavaSystem.GetProperty("http.agent");
                }
                return userAgent;
            }
        }
    }
}
