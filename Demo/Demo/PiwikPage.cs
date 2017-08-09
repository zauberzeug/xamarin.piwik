using System;
using System.Linq;
using Xamarin.Forms;

namespace Demo
{
    public class PiwikPage : ContentPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var path = "/" + string.Join("/", Navigation.NavigationStack.Select(p => p.Title).ToArray());
            //App.Analytics.TrackPage(Title, path);
        }
    }
}
