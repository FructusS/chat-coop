
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace mobile_app_on_signalr
{
    public partial class MainPage : ContentPage
    {
        public Command SendMessageCommand { get; }
        HubConnection hubConnection;
        Class1 Class1;
    
        //public ObservableCollection<MessageData> Messages { get; set; }
        public MainPage( string User)
        {


           // this.BindingContext = new Class1();
            InitializeComponent();






            Class1 = new Class1();
            Class1.UserName = User; 
            this.BindingContext = Class1;
           // Class1 = new Class1();

            // //hubConnection = new HubConnectionBuilder().WithUrl("https://10.0.2.2/chatHub").Build();
            // //hubConnection.On<string, string>("SendPrivate", (user, message) =>
            // //{
            // //    Messages.Add(new MessageData() { User = user, Message = message});
            // //});
            // hubConnection = new HubConnectionBuilder()
            //.WithUrl($"https://10.0.2.2:7007/chatHub", (opts) =>
            //{
            //    opts.HttpMessageHandlerFactory = (message) =>
            //    {
            //        if (message is HttpClientHandler clientHandler)
            //            // bypass SSL certificate
            //            clientHandler.ServerCertificateCustomValidationCallback +=
            //                (sender, certificate, chain, sslPolicyErrors) => { return true; };
            //        return message;
            //    };
            //})
            //.Build();
            // User = "User";
            // Message = "Message";
            // SendMessageCommand = new Command(async () => { await SendMessage(User, Message); });


        }
   
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Class1.Connect();
        }   
        //private async Task SendMessage()
        //{
        //    var text = message.Text;

        //    await hubConnection.SendAsync("SendPrivate", "123", message);


        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await Class1.Disconnect();
        }


        //}
        //public async Task GetMessages()
        //{
        //    hubConnection.On<MessageData>("SendPrivate", (message) =>
        //    {
        //        Messages.Add(message);

        //    });
        //}
        //private async void Button_Clicked(object sender, EventArgs e)
        //{
        //    SendMessage();
        //}
        private async Task SendMessage(string user, string message)
        {
            await hubConnection.InvokeAsync("SendMessage", user, message);

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            messagetext.Text = "";
        }
    }

 
}