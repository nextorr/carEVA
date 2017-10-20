using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Microsoft.WindowsAzure.MediaServices.Client;

using carEVA.Utils;


namespace carEVA.Controllers.API
{
    //this class upload the video from the client to the server file system
    public class UploadVideoController : ApiController
    {

        // Enable both Get and Post so that our jquery call can send data, and get a status
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage uploadVideo()
        {
            HttpPostedFile httpFile = null;
            string fileFullPath;
            try
            {
                // Get a reference to the file that our jQuery sent.  Even with multiple files, they will all be on their own request and be the 0 index
                httpFile = HttpContext.Current.Request.Files[0];
                //save the file in file system so we can use AMS file upload
                fileFullPath = fileUtils.saveFileToSystem(httpFile);
            }
            catch (Exception e)
            {
                evaLogUtils.logErrorMessage("error storing the file on the server",
                    "error", e, this.ToString(), nameof(this.uploadVideo));
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            //this is used to get form the user interface if there already exist a published video
            string currentURL = HttpContext.Current.Request.Form.Get(0);
            // here we are using a unique identifier to save the file
            //and prevent possible name conflict issues
            
            // Now we need to wire up a response so that the calling script understands what happened
            HttpContext.Current.Response.ContentType = "text/plain";
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new { name = httpFile.FileName, location = fileFullPath };

            HttpContext.Current.Response.Write(serializer.Serialize(result));
            HttpContext.Current.Response.StatusCode = 200;

            // For compatibility with IE's "done" event we need to return a result as well as setting the context.response
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
