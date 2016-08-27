using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class evaLog
    {
        public int evaLogID { get; set; }
        public string level { get; set; }
        public string caller { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }

    }
}