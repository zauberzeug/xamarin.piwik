using System;

using Xamarin.Forms;
using Xamarin.Piwik;

namespace Demo
{
    public class App : Application
    {
        public static Analytics Analytics = new Analytics("https://tracktest.365farmnet.com", 3);

        public App()
        {
            var content = new ContentPage {
                Title = "Demo",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Xamarin.Piwik Tracker Demo App"
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
