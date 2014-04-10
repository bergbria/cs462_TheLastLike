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
        static JArray jsonData;
        static string debug;
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
                jsonData = changed_values;
                /*debug = "0";
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                debug = "1";
                var user = userManager.FindById(User.Identity.GetUserId());
                debug = "2";
                List<string> likes = FacebookUtils.GetMusicLikes(user.FacebookAccessToken);
                debug = "3";
                LastFmUtils.AddTopHitsToPlaylist(user.LastFmSessionKey, user.LastFmPlaylistId, likes);
                debug = "4";
                FacebookUtils.PostToFacebook(user.FacebookAccessToken, "Last.fm playlist updated!!");
                debug = "5";*/
                return changed_values.ToString();
            }
        }
        
        public string Json()
        {
            return jsonData.ToString();  
        }

        public string Debug()
        {
            return debug;
        }

	}
}