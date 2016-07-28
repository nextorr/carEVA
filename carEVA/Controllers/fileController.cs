using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using carEVA.Models;

namespace carEVA.Controllers
{
    public class fileController : Controller
    {
        private carEVAContext db = new carEVAContext();
       
        // GET: file
        //return the corresponding list of uploaded files to the user
        public ActionResult Index(int? courseID, int? chapterID, int? lessonID)
        {
            if(courseID != null)
            {
                var files = db.Files.Where(c => c.courseID == courseID);
                ViewBag.viewType = "groupItems";
                if(!files.Any())
                {
                    //the query does not return any data, send a dummy object
                    List<evaFile> tempList = files.ToList();
                    tempList.Add(new evaFile()
                    {
                        evaFileID = -1,
                        Course = db.Courses.Where(c => c.CourseID == courseID).FirstOrDefault(),
                        courseID = (int)courseID
                    });
                    return View(tempList);
                }
                return View(files.ToList());
            }
            if (chapterID != null)
            {
                var files = db.Files.Where(c => c.chapterID == chapterID);
                ViewBag.viewType = "groupItems";
                if (!files.Any())
                {
                    //the query does not return any data, send a dummy object
                    List<evaFile> tempList = files.ToList();
                    var query = db.Chapters.Where(c => c.ChapterID == chapterID).Include(c => c.Course);
                    tempList.Add(new evaFile()
                    {
                        evaFileID = -1,
                        Chapter = query.FirstOrDefault(),
                        Course = query.FirstOrDefault().Course,
                        chapterID = (int)chapterID
                    });
                    return View(tempList);
                }
                return View(files.ToList());
            }
            if (lessonID != null)
            {
                var files = db.Files.Where(c => c.lessonID == lessonID);
                ViewBag.viewType = "groupItems";
                if (!files.Any())
                {
                    //the query does not return any data, send a dummy object
                    List<evaFile> tempList = files.ToList();
                    var query = db.Lessons.Where(c => c.LessonID == lessonID).Include(c => c.Chapter.Course);
                    tempList.Add(new evaFile()
                    {
                        evaFileID = -1,
                        Lesson = query.FirstOrDefault(),
                        Chapter = query.FirstOrDefault().Chapter,
                        Course = query.FirstOrDefault().Chapter.Course,
                        lessonID = (int)lessonID
                    });
                    return View(tempList);
                }
                return View(files.ToList());
            }
            //if all the parameters are null, return the full list
            var allFiles = db.Files;
            ViewBag.viewType = "allItems";
            return View(allFiles.ToList());
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, int? courseID, int? chapterID, int? lessonID)
        {
            //check that the request contains at least one ID
            if(courseID == null && chapterID == null && lessonID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                String fileName = Path.GetFileName(file.FileName);
                
                // store the file inside ~/App_Data/uploads folder
                //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                //file.SaveAs(path);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["StorageConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("files");
                //this is to ensure every uploaded file gets a unique name
                //like  with 0 chance of repeating
                Guid nameSuffix;
                string uniqueName="";
                CloudBlockBlob blockBlob = null;
                bool loopControl = true;
                while (loopControl)
                {
                    nameSuffix = Guid.NewGuid();
                    uniqueName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ","-") 
                        +"-"+ nameSuffix.ToString() + Path.GetExtension(file.FileName);
                    blockBlob = container.GetBlockBlobReference(uniqueName);
                    if (!blockBlob.Exists())
                    {
                        loopControl = false;
                    }
                }

                blockBlob.UploadFromStream(file.InputStream);

                //here the IDs are nullable, 
                //so if at least one ID exist is a valid model
                evaFile fileProxy = new evaFile()
                {
                    evaFileID = 0,
                    courseID = courseID,
                    chapterID = chapterID,
                    lessonID = lessonID,
                    fileName = fileName,
                    fileURL = blockBlob.Uri.ToString(),
                    fileStorageName = uniqueName
                };
                db.Files.Add(fileProxy);
                db.SaveChanges();

                if(courseID != null)
                {
                    return RedirectToAction("Index", new {courseID = courseID });
                }
                if (chapterID != null)
                {
                    return RedirectToAction("Index", new { chapterID = chapterID });
                }
                if (lessonID != null)
                {
                    return RedirectToAction("Index", new { lessonID = lessonID });
                }
            }
            return RedirectToAction("Index");
        }
        // GET: Chapters/Delete/5
        public ActionResult Delete(int? id, int? courseID, int? chapterID, int? lessonID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaFile file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            //now handle the redirection
            if (courseID != null)
            {
                ViewBag.backToParameter = "courseID";
                ViewBag.backToID = courseID;
            }
            if (chapterID != null)
            {
                ViewBag.backToParameter = "chapterID";
                ViewBag.backToID = chapterID;
            }
            if (lessonID != null)
            {
                ViewBag.backToParameter = "lessonID";
                ViewBag.backToID = lessonID;
            }
            return View(file);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? courseID, int? chapterID, int? lessonID)
        {
            evaFile file = db.Files.Find(id);
            //remove the file from the cloud storage
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("files");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.fileStorageName);

            try
            {
                blockBlob.Delete();
            }
            catch (Exception)
            {
                //if we cant delete the cloud file dont delete the database entry
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                throw;
            }

            db.Files.Remove(file);
            db.SaveChanges();

            //now handle the redirection
            if (courseID != null)
            {
                return RedirectToAction("Index", new { courseID = courseID });
            }
            if (chapterID != null)
            {
                return RedirectToAction("Index", new { chapterID = chapterID });
            }
            if (lessonID != null)
            {
                return RedirectToAction("Index", new { lessonID = lessonID });
            }

            return RedirectToAction("Index");
        }
    }
}