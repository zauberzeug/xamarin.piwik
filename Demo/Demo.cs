using System;

using Xamarin.Forms;

namespace Demo
{
    public class App : Application
    {
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
