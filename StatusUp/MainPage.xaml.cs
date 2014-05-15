using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Data.Json;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Phone.UI.Input;
namespace StatusUp
{

    public sealed partial class MainPage :  Page
    {
        public static string user = "";
        public static string url = "https://shrouded-lake-1392.herokuapp.com";
        //public static string url = "http://localhost:5000";
        public  MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.SourcePageType.FullName == "StatusUp.MainPage")
            {
                e.Handled = true;
                // Create the message dialog and set its content
                var messageDialog = new MessageDialog("Close Application?");

                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                messageDialog.Commands.Add(new UICommand(
                    "Yes",
                    new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand(
                    "No",
                    new UICommandInvokedHandler(this.CommandInvokedHandler)));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                try
                {
                    messageDialog.ShowAsync();
                }
                catch { }
            }
        }
        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Yes")
                Application.Current.Exit();

        }
 
        protected override  void OnNavigatedTo(NavigationEventArgs e){

                getData();
        }
        public async void getData()
        {
  
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.GetAsync(url + "/allsites");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            JsonValue jsonValue = JsonValue.Parse(responseString);
            for (uint i = 0; i < jsonValue.GetArray().Count; i++)
            {
                string site = jsonValue.GetArray().GetObjectAt(i).GetNamedString("site");
                string twitter = jsonValue.GetArray().GetObjectAt(i).GetNamedString("twitter");
                if (site == null)
                    break;
                var t = Tuple.Create<string, string>(site, twitter);
                Services.allServices.Add(t);
            }
           
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            ring.IsActive = true;
            
            b1.IsEnabled = false;
            b2.IsEnabled = false;
            password.IsEnabled = false;
            username.IsEnabled = false;
            var values = new List<KeyValuePair<string, string>>{
                new KeyValuePair<string, string>("username",username.Text),
                new KeyValuePair<string, string>("password",password.Password)
            };

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(url + "/login", new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString.Split(' ').ElementAt(0).Equals("authenticated"))
            {
                user = username.Text;
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["username"] = username.Text;
                MessageDialog md = new MessageDialog(responseString.ToUpper());
                md.ShowAsync();
                Frame.Navigate(typeof(Services));
            }
            else
            {
                ring.IsActive = false;
                password.IsEnabled = true;
                username.IsEnabled = true;
                b1.IsEnabled = true;
                b2.IsEnabled = true;
                MessageDialog md = new MessageDialog(responseString.ToUpper());
                md.ShowAsync();
            }
        }


        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            ring.IsActive = true;
            password.IsEnabled = false;
            username.IsEnabled = false;
            b1.IsEnabled = false;
            b2.IsEnabled = false;
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username",username.Text),
                new KeyValuePair<string, string>("password",password.Password)
            };        

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(url + "/signup", new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString.Split(' ')[0].Equals("Authenticated"))
            {
                user = username.Text;
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["username"] = username.Text;
                MessageDialog md = new MessageDialog(responseString.ToUpper());
                md.ShowAsync();
                Frame.Navigate(typeof(AddService));
              
            }
            else
            {
                ring.IsActive = false;
                password.IsEnabled = true;
                username.IsEnabled = true;
                b1.IsEnabled = true;
                b2.IsEnabled = true;
                MessageDialog md = new MessageDialog(responseString.ToUpper());
                md.ShowAsync();
            }
        }

    }
}
