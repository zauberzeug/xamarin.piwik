using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Piwik
{
    public partial class Analytics
    {
        string userAgent;
        string UserAgent {
            get {
                if (userAgent == null) {
                    var webView = new UIWebView(CGRect.Empty);
                    userAgent = webView.EvaluateJavascript("navigator.userAgent");
                    webView.Dispose();
                }
                return userAgent;
            }
        }
    }
}
