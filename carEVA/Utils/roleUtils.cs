using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.Utils
{
    
    public static class evaRoles
    {
        public const string Admin = "Admin";
        public const string Instructor = "Instructor";
        public static string admin { get { return Admin; } }
        public static string user { get { return "User"; } }
        public static string instructor { get { return Instructor; } }
    }
}