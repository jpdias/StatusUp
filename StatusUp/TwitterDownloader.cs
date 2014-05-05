using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace ProfileTimelineView.Twitter
{
    public class TwitterDownloader : DataDownloader
    {

        private const string ConsumerKey = "uyszNBPwJdDD2VX8qNci8yctE";
        private const string ConsumerSecret = "KYvDF4IOS0G2QwcKd2hzW6ytfmRzpUj8k5yKipBb4ho7CNZgIN";

      
        private const string OAuth2Token = "https://api.twitter.com/oauth2/token";
        private const string StatusesUserTimeline = "https://api.twitter.com/1.1/statuses/user_timeline.json";

        
        private const string DateTimeFormat = "ddd MMM dd HH:mm:ss +0000 yyyy";

        
        public string ScreenName { get; set; }

        private async Task<string> GetAccessTokenAsync()
        {
           
            var credentials = BuildCredentials();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
         
            var content =
                new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });
            var response = await client.PostAsync(OAuth2Token, content);
            
            var json = JsonObject.Parse(await response.Content.ReadAsStringAsync());
            return json.GetNamedString("access_token");
        }

        private string BuildCredentials()
        {
            var original = string.Format("{0}:{1}", WebUtility.UrlEncode(ConsumerKey),
                WebUtility.UrlEncode(ConsumerSecret));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(original));
        }

        private DateTime ParseCreatedString(string created)
        {
            var dst = TimeSpan.FromHours(TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? 1 : 0);
            return DateTime.ParseExact(created.Trim(), DateTimeFormat, new CultureInfo("en-us"))
                .Add(TimeZoneInfo.Local.BaseUtcOffset.Add(dst));
        }

        public override async Task<List<TimelineData>> GetTimelineAsync()
        {
           
            var token = await GetAccessTokenAsync();
            var client = new HttpClient { MaxResponseContentBufferSize = 1024 * 1024 };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
           
            var response = await client.GetAsync(string.Format("{0}?screen_name={1}",
                StatusesUserTimeline, WebUtility.UrlEncode(ScreenName)));
           
            var json = JsonValue.Parse(await response.Content.ReadAsStringAsync());
            var statuses = json.GetArray();
            return statuses.Select(status => status.GetObject())
                .Select(status =>
                {
                    var text = status.GetNamedString("text");
                    var user = status.GetNamedObject("user");
                    var created = status.GetNamedString("created_at");
                    var name = user.GetNamedString("name");
                    var screenname = user.GetNamedString("screen_name");
                    var profileImageUrl = user.GetNamedString("profile_image_url_https");
                    return new TweetData
                    {
                        Title = string.Format("{0} / @{1}", name, screenname),
                        Body = text,
                        ImageUri = new Uri(profileImageUrl),
                        Name = name,
                        ScreenName = screenname,
                        CreatedAt = ParseCreatedString(created)
                    };
                })
                .Cast<TimelineData>()
                .ToList();
        }

        public static DataDownloader Create(string screenName)
        {
            var downloader = new TwitterDownloader { ScreenName = screenName };
            return downloader;
        }
    }
}