using _462_TheLastLike.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using _462_TheLastLike.Utils.Facebook;
using _462_TheLastLike.Utils.LastFm;

namespace _462_TheLastLike.Controllers
{
    public class HomeController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ApplicationUser GetCurrentUser()
        {
            return UserManager.FindById(User.Identity.GetUserId());
        }

        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ActionResult Index()
        {
            //if (Request.IsAuthenticated)
            //{
            //    var user = GetCurrentUser();
            //    string accessToken = user.FacebookAccessToken;
            //    var likedArtists = FacebookUtils.GetMusicLikes(accessToken);
            //    FacebookUtils.PostToFacebook(accessToken, "please ignore this");
            //    var artist = LastFmUtils.FindArtist("carly rae jepsen");
            //}
            if (Request.IsAuthenticated)
            {
                var user = GetCurrentUser();
                string accessToken = user.FacebookAccessToken;
                ViewBag.isConnectedToLastFm = user.LastFmSessionKey != null;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LastFmCallback()
        {
            string token = Request.QueryString.Get("token");
            string sessionKey = LastFmUtils.ObtainSessionKey(token);
            string playlistId = LastFmUtils.CreatePlaylist("My LastLike Tracks", sessionKey, token);

            ApplicationUser user = GetCurrentUser();
            user.LastFmSessionKey = sessionKey;
            user.LastFmPlaylistId = playlistId;
            ApplicationDbContext.SaveChanges();

            var artistNames = FacebookUtils.GetMusicLikes(user.FacebookAccessToken);
            LastFmUtils.AddTopHitsToPlaylist(sessionKey, playlistId, artistNames);

            return RedirectToAction("Index");
        }
    }
}