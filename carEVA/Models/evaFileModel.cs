using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    
    public class evaFile
    {
        public int evaFileID { get; set; }
        [DisplayName("Nombre del archivo")]
        public string fileName { get; set; }
        public string fileStorageName { get; set; }
        [DisplayName("Ubicacion")]
        public string fileURL { get; set; }
        //this because filtering included data in EF is kinda awkard
        public int? courseID { get; set; }
        public int? chapterID { get; set; }
        public int? lessonID { get; set; }
        public virtual Course Course { get; set; }
        public virtual Chapter Chapter { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
    public class evaImage
    {
        public int evaImageID{ get; set; }
        [DisplayName("Nombre de la imagen")]
        public string imageName { get; set; }
        public string imageStorageName { get; set; }
        [DisplayName("Ubicacion")]
        public string imageURL { get; set; }
        
    }
    //public class CourseFile
    //{
    //    public int CourseFileID { get; set; }
    //    public int courseID { get; set; }
    //    public int fileID { get; set; }
    //    public virtual evaFile file {get; set;}
    //    public virtual Course Course { get; set; }

    //}
    //public class ChapterFile
    //{
    //    public int ChapterFileID { get; set; }
    //    public int chapterID { get; set; }
    //    public int fileID { get; set; }
    //    public virtual evaFile file { get; set; }
    //    public virtual Course Course { get; set; }
    //}
    //public class LessonFile
    //{
    //    public int LessonFileID { get; set; }
    //    public int lessonID { get; set; }
    //    public int fileID { get; set; }
    //    public virtual evaFile file { get; set; }
    //    public virtual Course Course { get; set; }
    //}
    
}
