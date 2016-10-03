using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using System.IO;
using Microsoft.WindowsAzure.MediaServices.Client;
using carEVA.Models;
using System.Data.Entity;

namespace carEVA.Utils
{
    public class mediaThreadParams
    {
        public string fileLocation { get; set; }
        public int lessonID { get; set; }
    }
    public class evaMediaServices
    {
        //read the acces credentials from the web config
        private static readonly string mMediaServicesAccountName = ConfigurationManager.AppSettings["MediaServicesAccountName"];
        private static readonly string mMediaServicesAccountKey = ConfigurationManager.AppSettings["MediaServicesAccountKey"];

        //cache the acces credentials and the context
        private static CloudMediaContext mMediaContext = null;
        private static MediaServicesCredentials mMediaCredentials = null;

        //default constructor
        public evaMediaServices()
        {
            if (mMediaCredentials == null || mMediaContext == null)
            {
                //create and cache the media service credentials
                mMediaCredentials = new MediaServicesCredentials(mMediaServicesAccountName, mMediaServicesAccountKey);
                //then create the media context
                mMediaContext = new CloudMediaContext(mMediaCredentials);
            }
        }

        public void uploadVideoToAzure(string fileFullPath, int _lessonID)
        {
            //Uri publishLocation;
            //IAsset fileAsset = UploadFile(fileFullPath, AssetCreationOptions.None);
            ////and then publish the file so it can be viewed on android
            //publishLocation = publishAndGetUrl(fileAsset);
            //return publishLocation.ToString();

            Thread th = new Thread(uploadAndSaveToDbThread);
            th.Start(new mediaThreadParams()
            {
                fileLocation = fileFullPath,
                lessonID = _lessonID
            });

            //uploadAndSaveToDbThread(new mediaThreadParams()
            //{
            //    fileLocation = fileFullPath,
            //    lessonID = _lessonID
            //});
        }

        private void uploadAndSaveToDbThread(Object _params)
        {
            mediaThreadParams inputParams;
            carEVAContext context = new carEVAContext();
            try
            {
                inputParams = (mediaThreadParams)_params;
            }
            catch (InvalidCastException)
            {
                //the parameters are invalid, log this information
                context.evaLogs.Add(new evaLog()
                {
                    level = "ERROR",
                    message = "Exception while casting the response: " + _params.ToString(),
                    caller = "Thread Error",
                    date = DateTime.Now
                });
                context.SaveChanges();
                return;
            }
            //read the entry tp update
            Lesson lesson = context.Lessons.Find(inputParams.lessonID);
            if (lesson == null)
            {
                //the lesson ID is invalid, log this information
                context.evaLogs.Add(new evaLog()
                {
                    level = "ERROR",
                    message = "the lesson ID does not exists" + inputParams.ToString(),
                    caller = "Thread Error",
                    date = DateTime.Now
                });
                context.SaveChanges();
                return;
            }

            //try to upload the video to azure
            Uri publishLocation;
            string videoName;
            string videoStorage;
            try
            {
                IAsset fileAsset = UploadFile(inputParams.fileLocation, AssetCreationOptions.None);
                //and then publish the file so it can be viewed on android
                videoName = fileAsset.Name;
                videoStorage = fileAsset.StorageAccountName;
                publishLocation = publishAndGetUrl(fileAsset);
            }
            catch (Exception e)
            {
                string exceptionMessage = "Exception: ";
                while (e != null)
                {
                    exceptionMessage += e.Message + " -- ";
                    e = e.InnerException;
                }
                //an error ocurred while trying to save to azure
                context.evaLogs.Add(new evaLog()
                {
                    level = "ERROR",
                    message = "Could not upload or publish the video on azure" + exceptionMessage,
                    caller = "Thread Error",
                    date = DateTime.Now
                });
                context.SaveChanges();
                return;
            }

            //save the metadata to the database
            lesson.videoURL = publishLocation.AbsoluteUri;
            lesson.videoName = videoName;
            lesson.videoStorageName = videoStorage;
            context.Entry(lesson).State = EntityState.Modified;
            context.SaveChanges();
            //then delete the file
            File.Delete(inputParams.fileLocation);
        }

        //helper method to control the uploading of the file and eventually its progress
        //see https://azure.microsoft.com/en-us/documentation/articles/media-services-dotnet-get-started/
        public IAsset UploadFile(string fileName, AssetCreationOptions options)
        {
            IAsset asset = mMediaContext.Assets.CreateFromFile(
                fileName,
                options,
                (af, p) =>
                {
                    //Console.WriteLine("Uploading '{0}' - Progress: {1:0.##}%", af.Name, p.Progress);
                });
            return asset;
        }

        //helper method to publish the asset and get the URL and save it to the database.
        public Uri publishAndGetUrl(IAsset asset)
        {
            Uri filePublishLocations;
            // Publish the output asset by creating an Origin locator for adaptive streaming
            mMediaContext.Locators.Create(LocatorType.Sas, asset, AccessPermissions.Read, TimeSpan.FromDays(3650), DateTime.Now);

            //get the file reference to progressive download it
            IEnumerable<IAssetFile> mp4Files = asset.AssetFiles.ToList().Where(af => af.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase));

            //TODO: check the behaviour here. since there is no encodig we expect only one file URL here
            filePublishLocations = mp4Files.Select(af => af.GetSasUri()).FirstOrDefault();

            return filePublishLocations;

        }
    }
}