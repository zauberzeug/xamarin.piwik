using System;

using Xamarin.Forms;
using Xamarin.Piwik;

namespace Demo
{
    public class App : Application
    {
        public static Analytics Analytics;

        public App()
        {
            Analytics = new Analytics("https://requestb.in/12xhpcf1", 13);
            Analytics.Verbose = true;
            Analytics.AppUrl = "http://demoapp";

            var content = new PiwikPage {
                Title = "Demo",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Xamarin.Piwik Tracker Demo App\n\nBrowse to\nhttps://requestb.in/12xhpcf1?inspect\nto see the api calls"
                        }
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
