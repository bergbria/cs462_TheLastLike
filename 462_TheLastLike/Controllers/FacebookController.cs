using _462_TheLastLike.Models;
using _462_TheLastLike.Utils.Facebook;
using _462_TheLastLike.Utils.LastFm;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _462_TheLastLike.Controllers
{
    public class FacebookController : Controller
    {
        static string jsonData;
        //
        // GET: /FacebookSubscription/
        public string Index()
        {
            return null;
        }

        public string Subscription()
        {
            if (Request.HttpMethod == "GET")
            {
                string challenge = Request.QueryString.Get("hub.challenge");
                return challenge;
            }
            else
            {
                string jsonString = new StreamReader(Request.InputStream).ReadToEnd();
                JObject parsedResponse = JObject.Parse(jsonString);
                JArray changed_values = (JArray) parsedResponse["entry"];
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = userManager.FindById(User.Identity.GetUserId());
                jsonData = "1";
                List<string> likes = FacebookUtils.GetMusicLikes(user.FacebookAccessToken);
                jsonData = "2";
                LastFmUtils.AddTopHitsToPlaylist(user.LastFmSessionKey, user.LastFmPlaylistId, likes);
                jsonData = "3";
                FacebookUtils.PostToFacebook(user.FacebookAccessToken, "Last.fm playlist updated!!");
                jsonData = "4";
                return likes.ToString();
            }
        }
        
        public string Json()
        {
            return jsonData.ToString();  
        }


	}
}