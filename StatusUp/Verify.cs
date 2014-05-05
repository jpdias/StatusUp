using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;

namespace StatusUp
{
    public class ServerData
    {
        public HttpResponseHeaders headers { get; set; }
        public string url { get; set; }
        public int loadTime { get; set; }
        public string status { get; set; }
        public bool response { get; set; }
        public string ip { get; set; }
    }
    class Verify
    {
        public static List<ServerData> alldata = new List<ServerData>();

        public static readonly List<string> Sites = new List<string>
        {

           /* "http://dropbox.com/",
            "http://google.com/",
            "http://yahoo.com/",
            "http://jpdias.github.io",
            "http://microsoft.com",
            "http://fe.up.pt/",
            "http://www.up.pt/",
            "http://onedrive.com/",
            "http://fada.pt/" */
        };

        public static async Task DownList()
        {

            var values = new List<KeyValuePair<string, string>>{
                new KeyValuePair<string, string>("username",MainPage.user)
            };

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(MainPage.url + "/apps", new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var temp = JsonArray.Parse(responseString);
            Sites.RemoveRange(0, Sites.Count);
            foreach (var x in temp) {
                Sites.Add(x.GetString());
            }
        
            var downloadTasksQuery = Sites.Select(s => GetAsync(s));
            var downloadTasks = downloadTasksQuery.ToList();


            while (downloadTasks.Count > 0)
            {
                var firstFinishedTask = await Task.WhenAny(downloadTasks);

                downloadTasks.Remove(firstFinishedTask);
                var str = await firstFinishedTask;
                if (str != null)
                    alldata.Add(str);
            }
   
        }

        public static async Task<ServerData> GetAsync(string uri)
        {
            ServerData s1 = new ServerData();
            var httpClient = new HttpClient();
            //httpClient.Timeout = new TimeSpan(0,0,5);
            DateTime start = DateTime.Now;
            var response = await httpClient.GetAsync(uri);
            DateTime t = DateTime.Now;
            int tim = (t - start).Milliseconds;
            if(tim >= 2500 || !response.IsSuccessStatusCode){
              s1.response = false;
              s1.headers = response.Headers;
              s1.loadTime = 0;
              s1.status = response.StatusCode.ToString() + "";
              s1.url = uri;
              return await Task.Run(() => s1);
            }
            else{
                //s1.ip = response.
                s1.response = true;
                s1.headers = response.Headers;
                s1.loadTime = tim;
                s1.status = response.StatusCode.ToString()+" : " + response.ReasonPhrase.ToString();
                s1.url = uri;
            }
            return await Task.Run(() => s1);
        }
    }
}
