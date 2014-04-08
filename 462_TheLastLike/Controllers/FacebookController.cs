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
                JArray changed_values = (JArray) parsedResponse["changed_fields"];
                jsonData = changed_values;
                return changed_values.ToString();
            }
        }
        
        public string Json()
        {
            return jsonData.ToString();  
        }


	}
}