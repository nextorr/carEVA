﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using carEVA.Models;

namespace carEVA.ViewModels
{
    public class userCourseEnrollment
    {
        public string publicKey { get; set; }
        public int courseID { get; set; }
    }

    public class userChapterDetail
    {
        public int percentViewed { get; set; }
        public Chapter chapter { get; set; }
        public ICollection<userLessonDetail> lessons { get; set; }
    }

    public class userLessonDetail
    {
        public Lesson info  { get; set; }
        public evaLessonDetail userDetail { get; set; }
    }

    public class response
    {
        public int questionID { get; set; }
        public int answerID { get; set; }
    }

    public class quizResponses
    {
        public string publicKey { get; set; }
        public int lessonDetailID { get; set; }
        public ICollection<response> responses { get; set; }
    }
}