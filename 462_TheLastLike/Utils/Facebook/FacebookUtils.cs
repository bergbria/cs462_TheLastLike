using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _462_TheLastLike.Utils.Facebook
{
    public class FacebookUtils
    {
        private static WebClient CreateFacebookWebClient(string accessToken)
        {
            WebClient webClient = new WebClient();
            webClient.QueryString.Add("access_token", accessToken);
            return webClient;
        }

        /// <summary>
        /// retrieves a list of the names of the user's like musical artists
        /// </summary>
        /// <param name="accessToken">the user's facebook api access token</param>
        /// <returns>list of artist names</returns>
        public static List<string> GetMusicLikes(string accessToken)
        {
            string jsonResponse;
            using (WebClient webClient = CreateFacebookWebClient(accessToken))
            {
                jsonResponse = webClient.DownloadString(FacebookConstants.GET_MUSIC_LIKES);
            }
            dynamic parsedResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            IEnumerable<dynamic> likes = parsedResponse.data;
            return likes.Select(like => (string)like.name).ToList();
        }

        /// <summary>
        /// retrieves a list of the names of the user's like musical artists
        /// </summary>
        /// <param name="accessToken">the user's facebook api access token</param>
        /// <returns>list of artist names</returns>
        public static List<string> GetMusicLikes(string userId, string accessToken)
        {
            string jsonResponse;
            using (WebClient webClient = CreateFacebookWebClient(accessToken))
            {
                jsonResponse = webClient.DownloadString("https://graph.facebook.com/" + userId + "/music");
            }
            dynamic parsedResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            IEnumerable<dynamic> likes = parsedResponse.data;
            return likes.Select(like => (string)like.name).ToList();
        }


        /// <summary>
        /// Posts a message to the user's wall. May contain HTML (I think)
        /// </summary>
        /// <param name="accessToken">the user's facebook api access token</param>
        /// <param name="message">the message to post to the wall</param>
        public static void PostToFacebook(string accessToken, string message)
        {
            string url = string.Format(FacebookConstants.POST_MESSAGE, accessToken);
            using (WebClient webClient = CreateFacebookWebClient(accessToken))
            {
                var data = new NameValueCollection();
                data["message"] = message;
                webClient.UploadValues(url, "POST", data);
            }
        }
    }
}