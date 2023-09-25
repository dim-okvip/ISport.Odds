using System.Net;
using System.Text;

namespace ISport.Odds
{
    public static class Utils
    {
        public const string CORS_POLICY = "CorsPolicy";
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static string PreMatchAndInPlayOddsMainId = "64ffe293e95b5c58571473c1";
        public static string TotalCornersPreMatchId = "6502cadaf1dd013e2334aec4";
        public static string TotalCornersInPlayId = "6502cae3f1dd013e2334aec5";

        public static string SendGet(string url)
        {
            // Create request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // GET request
            request.Method = "GET";
            request.ReadWriteTimeout = 5000;
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

            // Return content
            string retString = myStreamReader.ReadToEnd();
            return retString;
        }

        public static string GetUriWithQueryString(string requestUri, Dictionary<string, string> queryStringParams)
        {
            bool startingQuestionMarkAdded = false;
            var sb = new StringBuilder();
            sb.Append(requestUri);
            foreach (var parameter in queryStringParams)
            {
                if (parameter.Value == null)
                {
                    continue;
                }

                sb.Append(startingQuestionMarkAdded ? '&' : '?');
                sb.Append(parameter.Key);
                sb.Append('=');
                sb.Append(parameter.Value);
                startingQuestionMarkAdded = true;
            }
            return sb.ToString();
        }

        public static T Clone<T>(this T self)
        {
            var serialized = JsonSerializer.Serialize(self);
            return JsonSerializer.Deserialize<T>(serialized);
        }
    }

    public enum Source
    {
        MongoDB,
        InMemory
    }

    public class InMemory
    {
        public static PreMatchAndInPlayOddsMain PreMatchAndInPlayOddsMain = new PreMatchAndInPlayOddsMain();
        public static TotalCorners TotalCornersPreMatch = new TotalCorners();
        public static TotalCorners TotalCornersInPlay = new TotalCorners();
    }
}
