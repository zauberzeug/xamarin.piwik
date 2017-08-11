using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Piwik;

namespace Demo
{
    public class App : Application
    {
        public static Analytics Analytics;

        public App()
        {
            Analytics = new Analytics("http://mockbin.org/bin/9ac466a2-313c-4691-bf74-f15b9ab602b2", 13);
            Analytics.Verbose = true;
            Analytics.AppUrl = "http://demoapp";

            var content = new PiwikPage {
                Title = "Demo",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Xamarin.Piwik Tracker Demo App\n\nBrowse to\nhttps://goo.gl/tHhz23\nto see the api calls"
                        },
                        new Button{
                            Text = "Open SubPage",
                            Command = new Command(() => {
                                MainPage.Navigation.PushAsync(new PiwikPage{
                                    Title = "SubPage",
                                });
                            }),
                        }
                    }
                }
            };

            var navigation = new NavigationPage(content);
            navigation.Pushed += (sender, e) => TrackCurrentPage();
            navigation.Popped += (sender, e) => TrackCurrentPage();
            MainPage = navigation;
        }

        protected override void OnStart()
        {
            TrackCurrentPage();
        }

        protected override void OnSleep()
        {
            Analytics.LeavingTheApp();
        }

        protected override void OnResume()
        {
            TrackCurrentPage();
        }

        void TrackCurrentPage()
        {
            var path = "/" + string.Join("/", MainPage.Navigation.NavigationStack.Select(p => p.Title).ToArray());
            Analytics.TrackPage(MainPage.Navigation.NavigationStack.First().Title, path);
        }
    }
}
