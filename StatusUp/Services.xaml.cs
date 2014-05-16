using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input;
using Windows.Phone.UI.Input;
using Windows.Storage;

namespace StatusUp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    
    public sealed partial class Services : Page
    {
        
        public static List<Tuple<String, String>> allServices = new List<Tuple<String, String>>();
        public Services()
        {
            this.InitializeComponent();
            CheckInternet();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }
        public async void CheckInternet()
        {
            while (!App.IsInternetAvailable)
            {


                var messageDialog = new MessageDialog(("No data connection has been found."));

                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                messageDialog.Commands.Add(new UICommand(
                    "Try again",
                    new UICommandInvokedHandler(this.CommandInvokedHandlerInternet)));
                messageDialog.Commands.Add(new UICommand(
                    "Close",
                    new UICommandInvokedHandler(this.CommandInvokedHandlerInternet)));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                try
                {
                   await messageDialog.ShowAsync();
                }
                catch { }
            }
             getAll();
             MainPage.getData();
        }
        public void CommandInvokedHandlerInternet(IUICommand command)
        {         
            if (command.Label == "Try again")
            {
             
            }
            else
            {
              Application.Current.Exit();
            }

        }

       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
        public async void getAll()
        {
          
                
                r1.IsEnabled = true;
                r1.IsActive = true;
                listBox1.Items.Clear();
                Verify.alldata.RemoveRange(0,Verify.alldata.Count);
                await Verify.DownList();
                r1.IsEnabled = false;
                r1.IsActive = false;

                for (int i = 0; i < Verify.alldata.Count; i++)
                {
                    SerDetailPage obj1 = new SerDetailPage();
                    if (Verify.alldata.ElementAt(i).response == false)
                    { obj1.img = "Images/cancel.png";
                    obj1.name = (new Uri(Verify.alldata.ElementAt(i).url).Host.ToUpper().Trim().Replace("WWW.", "")).ToString();
                    obj1.Lag = "";
                    obj1.Loc = "";
                    obj1.Server = "";
                    obj1.Age = "";
                    obj1.url = (string)Verify.alldata.ElementAt(i).url;
                    }
                    else
                    {
                        obj1.img = "Images/check.png";
                        obj1.name = (new Uri(Verify.alldata.ElementAt(i).url).Host.ToUpper().Trim().Replace("WWW.", "")).ToString();
                        obj1.Lag = Verify.alldata.ElementAt(i).loadTime + " ms";
                        obj1.Loc = Verify.alldata.ElementAt(i).headers.Connection.ToString() + "";
                        obj1.Server = Verify.alldata.ElementAt(i).headers.Server.ToString() + "";
                        obj1.Age = Verify.alldata.ElementAt(i).headers.Age.ToString() + "";
                        obj1.url = (string)Verify.alldata.ElementAt(i).url;
                        obj1.statuscode = Verify.alldata.ElementAt(i).status + "";
                    }
                    listBox1.Items.Add(obj1);
                    listBox1.SelectionChanged += listBox1_Click;
                    //listBox1.

                    /*TextBox t1 = new TextBox();
                    t1.Text =  +" => "+
                          + " => " +
                          Verify.res.ElementAt(i).Item3;*/
                    // servlist.Items.Add(l1);    
                }
        }


        async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.SourcePageType.FullName == "StatusUp.Services") { 
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
            if(command.Label == "Yes")
                Application.Current.Exit();
           
        }

        private bool inMyHandle = false;
        private void listBox1_Click(object sender, SelectionChangedEventArgs e)
        {

            if (((ListBox)sender).SelectedIndex != -1)
            {
                ListBox selectedItem = (ListBox)sender;
                SerDetailPage s1 = (SerDetailPage)selectedItem.SelectedItem;
                // reset selection of ListBox 
                ((ListBox)sender).SelectedIndex = -1;
                // change page navigation 
                Frame.Navigate(typeof(SerDetails), s1);
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Add));
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("username");
            Application.Current.Exit();
            
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            getAll();
        }           
         
    }
}
