using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.ViewModels
{
    public class evaResponses
    {
        public string message { get; set; }
        public string evaCode { get; set; }

        public evaResponses(string _message, string code) {
            message = _message;
            evaCode = code;
        }
    }
}