using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobile_app_on_signalr
{
    public partial class App : Application
    {
        public static Class1 chat;
        public App()
        {
            InitializeComponent();

           MainPage = new NavigationPage(new  WelcomePage());
         
        }

        protected override  void OnStart()
        {
        
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
