using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using _462_TheLastLike.Utils.LastFm.DO;
using System.Collections.Generic;

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

        private static string GenerateApiSignature(NameValueCollection arguments)
        {
            var parameters = arguments.AllKeys.OrderBy(s => s);
            var preHash = string.Join("",
                from parameter in parameters
                select parameter + arguments.Get(parameter)
                );
            preHash += ApiSecret;
            return CalculateMD5Hash(preHash);
        }

        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2").ToLowerInvariant());
            }
            return sb.ToString();
        }

        public static string ObtainSessionKey(string token)
        {
            string methodName = "auth.getSession";
            NameValueCollection arguments = new NameValueCollection
            {
                {"token", token},
                {"api_key", ApiKey},
                {"method", methodName},
            };
            var apiSig = GenerateApiSignature(arguments);
            arguments.Add("api_sig", apiSig);

            using (var webClient = new WebClient())
            {
                webClient.QueryString = arguments;
                webClient.QueryString.Add("format", "json");

                string responseData = webClient.DownloadString(LastFmUrls.API_ROOT);
                dynamic parsedResponse = JObject.Parse(responseData);
                return parsedResponse.session.key;
            }
        }

        private static string GenerateMethodSignature(string token, string method)
        {
            string apiSigPreHash = "api_key" + ApiKey + "method" + method + "token" + token + ApiSecret;
            string apiSig = CalculateMD5Hash(apiSigPreHash);
            return apiSig;
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

        public static Track[] GetTopTracks(string artistSearchText)
        {
            using (WebClient webClient = CreateLastFmWebClient("artist.getTopTracks"))
            {
                webClient.QueryString.Add(new NameValueCollection
                {
                    {"artist", artistSearchText},
                    {"limit", "5"},
                    {"autocorrect", "1"}
                });

                string jsonResponse = webClient.DownloadString(LastFmUrls.API_ROOT);
                JObject parsedResponse = JObject.Parse(jsonResponse);

                var matchingTracks = parsedResponse["toptracks"];
                if (matchingTracks == null || !matchingTracks.Any())
                {
                    return null;
                }
                var topTracksArr = JsonConvert.DeserializeObject<Track[]>(matchingTracks["track"].ToString());
                return topTracksArr;
            }
        }

        private static string JoinNvcToQs(NameValueCollection qs)
        {
            return string.Join("&", Array.ConvertAll(qs.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(qs[key]))));
        }

        //arguments should not include api key or format=json
        private static string PostToLastFM(NameValueCollection arguments, string sessionKey)
        {
            arguments.Add("api_key", ApiKey);
            arguments.Add("sk", sessionKey);
            string apiSig = GenerateApiSignature(arguments);
            arguments.Add("api_sig", apiSig);
            arguments.Add("format", "json");

            using (var hc = new HttpClient())
            {
                var requestBody = JoinNvcToQs(arguments);
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                hc.DefaultRequestHeaders.ExpectContinue = false;

                var task = hc.PostAsync(LastFmUrls.API_ROOT, content);
                task.Wait();
                var response = task.Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static string CreatePlaylist(string name, string sessionKey, string token)
        {
            string methodName = "playlist.create";

            NameValueCollection arguments = new NameValueCollection
                {
                    {"method", methodName},
                    {"title", name},
                    {"description", "Generated for you by The Last Like"},
                };

            string result = PostToLastFM(arguments, sessionKey);
            dynamic parsedResult = JsonConvert.DeserializeObject<dynamic>(result);
            string playlistId = parsedResult.playlists.playlist.id;
            return playlistId;
        }

        public static void AddTop5TracksToPlaylist(string sessionKey, string playlistId, string artistName)
        {
            Track[] top5 = GetTopTracks(artistName);

            foreach (var track in top5)
            {
                NameValueCollection arguments = new NameValueCollection
                    {
                        {"method", "playlist.addTrack"},
                        {"playlistID", playlistId},
                        {"track", track.name},
                        {"artist", artistName},
                    };
                string response = PostToLastFM(arguments, sessionKey);
                response.ToString();
            }

        }

        public static void AddTopHitsToPlaylist(string sessionKey, string playlistId, IEnumerable<string> artistNames )
        {
            foreach (var artistName in artistNames)
            {
                AddTop5TracksToPlaylist(sessionKey, playlistId, artistName);
            }
        }
    }
}