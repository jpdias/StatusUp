using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace ProfileTimelineView
{
    public abstract class DataDownloader
    {
        public abstract Task<List<TimelineData>> GetTimelineAsync();

        protected string BuildUri(string uri, IDictionary<string, string> parameters)
        {
            var query = parameters.Select(p => string.Format("{0}={1}", WebUtility.UrlEncode(p.Key), WebUtility.UrlEncode(p.Value)));
            return string.Format("{0}?{1}", uri, string.Join("&", query));
        }

        protected string BuildUri(string uri, object parameters)
        {
            var type = parameters.GetType();
            var props = type.GetRuntimeProperties();
            var query = props.Where(prop => prop.PropertyType == typeof(string))
                .Select(prop => new { Key = prop.Name, Value = (string)prop.GetValue(parameters) })
                .Select(p => string.Format("{0}={1}", WebUtility.UrlEncode(p.Key), WebUtility.UrlEncode(p.Value)));
            return string.Format("{0}?{1}", uri, string.Join("&", query));
        }
    }
}