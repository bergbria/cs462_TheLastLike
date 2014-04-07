using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using _462_TheLastLike.Utils.LastFm.DO;

namespace _462_TheLastLike.Utils.LastFm
{
    public class LastFmUtils
    {
        public const string ApiKey = "b06a27c2149690731f77c269f1810928";
        public const string ApiSecret = "98b1814c8e12a1a5a651994c1a16401b";

        private static WebClient CreateLastFmWebClient(string method)
        {
            return new WebClient
            {
                QueryString = new NameValueCollection
                {
                    {"format", "json"},
                    {"api_key", ApiKey},
                    {"method", method}
                }
            };
        }
        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2").ToLowerInvariant());
            }
            return sb.ToString();
        }

        public static string ObtainSessionKey(Uri callbackUrl)
        {
            string token = HttpUtility.ParseQueryString(callbackUrl.Query).Get("token");

            string apiSigPreHash = "api_key" + ApiKey + "methodauth.getSessiontoken" + token + ApiSecret;
            string apiSig = CalculateMD5Hash(apiSigPreHash);

            using (var webClient = CreateLastFmWebClient("auth.getSession"))
            {
                webClient.QueryString.Add(new NameValueCollection
                {
                    {"token", token},
                    {"api_sig", apiSig},
                });

                string responseData = webClient.DownloadString(LastFmUrls.API_ROOT);
                dynamic parsedResponse = JObject.Parse(responseData);
                return parsedResponse.session.key;
            }
        }

        public static Artist FindArtist(string artistSearchText)
        {
            using (WebClient webClient = CreateLastFmWebClient("artist.search"))
            {
                webClient.QueryString.Add(new NameValueCollection
                {
                    {"artist", artistSearchText},
                    {"limit", "1"},
                });

                string jsonResponse = webClient.DownloadString(LastFmUrls.API_ROOT);
                JObject parsedResponse = JObject.Parse(jsonResponse);

                var matchingArtists = parsedResponse["results"]["artistmatches"];

                if (matchingArtists == null || !matchingArtists.Any())
                {
                    return null;
                }

                Artist artist = JsonConvert.DeserializeObject<Artist>(matchingArtists["artist"].ToString());

                return artist;
            }
        }
    }
}