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
           

            getAll();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Button 1 works!");
            //Do work for your application here.
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
          
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
         
    }
}
