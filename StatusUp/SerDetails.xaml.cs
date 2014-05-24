using StatusUp.Common;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Net.Http;
using System.Net;
using Windows.Web.Http.Headers;
using ProfileTimelineView;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace StatusUp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class Tweet
    {
        public string created_at { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        // Add more fields in here if you need them
    }
    public class SerDetailPage
    {
        public string url { get; set; }
        public string Server { get; set; }
        public string Age { get; set; }
        public string Lag { get; set; }
        public string Loc { get; set; }
        public string img { get; set; }
        public string name { get; set; }
        public string statuscode { get; set; }
    }

    public sealed partial class SerDetails : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public SerDetails()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.RegisterForShare();

            
        }
        public  static  SerDetailPage s = new SerDetailPage();
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            s = e.Parameter as SerDetailPage;
            ptitle.Title = s.name.ToUpper();
            Age.Text = s.Age;
            lag.Text = s.Lag;
            loc.Text = s.Loc;
            Server.Text = s.Server;
            if(s.statuscode!=null)
             stat.Text = s.statuscode;
           
            var temp = Services.allServices.Find(item => item.Item1.Equals(s.url));
            if (temp == null)
            {
                  temp = Services.allServices.Find(item => item.Item1.Equals(s.url+"/"));
            }
            if (temp == null)
            {
                string str = s.url.Insert(7, "www.");
                temp = Services.allServices.Find(item => item.Item1.Equals(str));
            }
            if (temp == null)
            {
                string str1 = s.url.Insert(7, "www.");
                temp = Services.allServices.Find(item => item.Item1.Equals(str1+"/"));
            }
            if (temp != null) { 

                DataDownloader t1 = ProfileTimelineView.Twitter.TwitterDownloader.Create(temp.Item2);
                ring1.IsEnabled = true;
                ring1.IsActive = true;
                //listBox1.Items.Clear();
                List<TimelineData> ltw = (await Task.WhenAll(t1.GetTimelineAsync()))[0];
                ring1.IsEnabled = false;
                ring1.IsActive = false;



                for (int i = 0; i < ltw.Count; i++)
                {
                    listBox1.Items.Add(ltw.ElementAt(i));
                }
            }
            else
            {
                ptitle.Items.RemoveAt(1);
            }
        }
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
           
        }

         
        #endregion

        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);
        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            // The Title is mandatory
            request.Data.Properties.Title = "StatusUpdate Info: " + s.name;
            request.Data.Properties.Description = "StatusUpdate Application";

            Age.Text = s.Age;
            lag.Text = s.Lag;
            loc.Text = s.Loc;
            stat.Text = s.statuscode;
            Server.Text = s.Server;
            string msg = "Status: " + s.statuscode + "\n" +
                "Server: " + s.Server + "\n" +
                "Loading Time: " + s.Lag + "\n";
            if (s.Loc != null)
                msg += "Location: " + s.Loc + "\n";
            if(s.Age !="")
                msg += "Age: " + s.Age + "\n";
            request.Data.SetText(msg);
        }
        private void share_Click(object sender, RoutedEventArgs e)
        {

            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private async void rm_Click(object sender, RoutedEventArgs e)
        {
            var values = new List<KeyValuePair<string, string>>{
                new KeyValuePair<string, string>("username",MainPage.user),
                new KeyValuePair<string, string>("service",s.url)
            };

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(MainPage.url + "/remove", new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var messageDialog = new MessageDialog(responseString.ToUpper());
            messageDialog.ShowAsync();
            Frame.Navigate(typeof(Services));
        }
    }
}
