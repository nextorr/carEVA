using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace carEVA.Utils
{
    public static class fileUtils
    {
        //file storage parameters
        private static readonly string serverMapPath = "~/Files/videos/";
        private static string storageRoot = Path.Combine(HostingEnvironment.MapPath(serverMapPath));
        //private string storageRoot { get { return Path.Combine(HostingEnvironment.MapPath(serverMapPath)); } }
        //since the AMS has a convenient method to upload from filesystem, save the received file in disk
        public static string saveFileToSystem(HttpPostedFile file)
        {
            string pathOnServer = Path.Combine(storageRoot);
            if (!Directory.Exists(pathOnServer))
            {
                Directory.CreateDirectory(pathOnServer);
            }
            string fullPath = Path.Combine(pathOnServer, Path.GetFileName(file.FileName));
            //this overwrites if the file exists, which is nice, i think
            file.SaveAs(fullPath);
            return fullPath;
        }
    }

    //TODO: delete the files once hey are uploaded to azure
}