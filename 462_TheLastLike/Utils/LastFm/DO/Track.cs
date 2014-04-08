using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _462_TheLastLike.Utils.LastFm.DO
{
    public class Track
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string playcount { get; set; }
        public string listeners { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public Artist artist { get; set; }
    }
}