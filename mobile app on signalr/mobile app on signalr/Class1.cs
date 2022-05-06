using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace mobile_app_on_signalr
{

    public class Class1 : INotifyPropertyChanged
    {
        HttpClient client;
        HttpClientHandler clientHandler;
        HubConnection hubConnection;

        public string UserName { get; set; }
        public string Message { get; set; }
        // список всех полученных сообщений
        public ObservableCollection<MessageData> Messages { get; set; }


        public string messagesList = $"https://10.0.2.2:7288/api/Messages";





        public Command SendMessageCommand { get; }


        public Class1()
        {
            Messages = new ObservableCollection<MessageData>();
            SendMessageCommand = new Command(async () => { await SendMessage(); });
            clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            client = new HttpClient(clientHandler);
            
            hubConnection = new HubConnectionBuilder()
                        .WithUrl($"http://chatcoopsignalr.somee.com/chatHub", (opts) =>
                                {
                                    opts.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
        (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        return message;
                    };
                                })
                                .Build();
        //           .WithUrl($"https://10.0.2.2:7288/chatHub", (opts) =>
        //                        {
        //                            opts.HttpMessageHandlerFactory = (message) =>
        //            {
        //                if (message is HttpClientHandler clientHandler)
        //                    // bypass SSL certificate
        //                    clientHandler.ServerCertificateCustomValidationCallback +=
        //(sender, certificate, chain, sslPolicyErrors) => { return true; };
        //                return message;
        //            };
        //                        })
        //                        .Build();
           
            hubConnection.Closed += async (error) =>
            {

                try
                {
                    await Task.Delay(3000);
                    await Connect();
                }
                catch
                {

                }
                   
              
       

            };


            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
              
                SendLocalMessage(user, message);
            });


            hubConnection.On<string>("Join", (user) =>
            {
               
                SendLocalMessage(user,  "только что подключился!");
            });
            hubConnection.On<string>("Leave", (user) =>
            {
            
                SendLocalMessage(user, "только что отключился!");
            });

        }
        async Task SendMessage()
        {
            try
            {

                await hubConnection.InvokeAsync("SendMessage", UserName, Message);
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }

        }
        private void SendLocalMessage(string user, string message)
        {
            Messages.Insert(0, new MessageData
            {
                Message = message,
                User = user
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public async Task Connect()
        {
         

            try
            {
                await this.hubConnection.StartAsync();
              //  await GetMessages();
                await hubConnection.InvokeAsync("JoinChat", UserName);
            }
                catch(Exception ex)
            {
                SendLocalMessage(String.Empty, "не удалось подключиться \n" + ex.Message);
            }
                // yay! connected
               
          
            //await hubConnection.StartAsync();
            // await hubConnection.InvokeAsync("JoinChat", User);


        }
        public async Task GetMessages()
        {
            //try
            //{
            //    var response = await client.GetAsync(messagesList);
            //    var content = await response.Content.ReadAsStringAsync();
            //    Messages = JsonSerializer.Deserialize<ObservableCollection<MessageData>>(content);

            //}
            //catch(Exception ex)
            //{
            //    SendLocalMessage(String.Empty, ex.Message);
            //}
            try
            {
                Messages = await hubConnection.InvokeAsync<ObservableCollection<MessageData>>("GetMessageHistory");

            }

            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, ex.Message);
            }

        }

        public async Task SendMessage(string user, string message)
        {
            await hubConnection.InvokeAsync("SendMessage", user, message);
        }

        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("LeaveChat", UserName);
            //await hubConnection.InvokeAsync("LeaveChat", User);
            await hubConnection.StopAsync();



        }
    }

}
