using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace carEVA.Utils
{
    //this is used because  the default dpapi does not work on azure
    // see http://stackoverflow.com/questions/23455579/generating-reset-password-token-does-not-work-in-azure-website
    public class evaDataProtectionProvider : IDataProtectionProvider
    {
        
        public IDataProtector Create(params string[] purposes)
        {
            return new evaDataProtector(purposes);
        }
    }

    public class evaDataProtector : IDataProtector
    {
        private readonly string[] _purposes;

        public evaDataProtector(string[] purposes)
        {
            _purposes = purposes;
        }

        public byte[] Protect(byte[] userData)
        {
            return MachineKey.Protect(userData, _purposes);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return MachineKey.Unprotect(protectedData, _purposes);
        }
    }
}