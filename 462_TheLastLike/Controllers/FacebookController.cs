using _462_TheLastLike.Models;
using _462_TheLastLike.Utils.Facebook;
using _462_TheLastLike.Utils.LastFm;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                JArray entries = (JArray) parsedResponse["entry"];
                jsonData = entries;
                debug = "0";
                ApplicationDbContext con = new ApplicationDbContext();
                IDbSet<ApplicationUser> users = con.Users;
                debug = users.ToString();
                /*foreach (var entry in entries) {
                    foreach (var user in users) 
                    {
                        if (user.FacebookUserId == (string) entry["id"])
                        {
                            List<string> likes = FacebookUtils.GetMusicLikes(user.FacebookUserId, user.FacebookAccessToken);
                            LastFmUtils.AddTopHitsToPlaylist(user.LastFmSessionKey, user.LastFmPlaylistId, likes);
                        }
                    }
                }*/
                debug = "1";
                /*
                debug = "3";
                debug = "4";
                FacebookUtils.PostToFacebook(user.FacebookAccessToken, "Last.fm playlist updated!!");
                debug = "5";*/
                return entries.ToString();
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