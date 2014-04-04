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

namespace _462_TheLastLike.Controllers
{
    public class HomeController : Controller
    {
        private const string LastFMAPIKey = "b06a27c2149690731f77c269f1810928";
        private const string LastFMAPISecret = "98b1814c8e12a1a5a651994c1a16401b";

        public ActionResult Index()
        {
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

        public ActionResult LastFMCallback()
        {
            string token = HttpUtility.ParseQueryString(Request.Url.Query).Get("token");
            var db = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = userManager.FindById(User.Identity.GetUserId());

            string apiSigPreHash = "api_key" + LastFMAPIKey + "methodauth.getSessiontoken" + token + LastFMAPISecret;
            string apiSig = Lastfm.Utilities.md5(apiSigPreHash);

            string queryString =
                "&token=" + token +
                "&api_key=" + LastFMAPIKey +
                "&api_sig=" + apiSig +
                "&format=json";

            string baseURL = "http://ws.audioscrobbler.com/2.0/?method=auth.getSession";
            string url = baseURL + queryString;
            
            WebRequest request = WebRequest.Create(url);

            var response = request.GetResponse();
            string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

            dynamic foo = JObject.Parse(responseData);
            string sessionKey = foo.session.key;

            user.LastFMSessionKey = token;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}