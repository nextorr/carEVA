using System;
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
    public class userQuizDetail
    {
        public bool viewed { get; set; }
        public bool passed { get; set; }
        public int totalObtainedPoints { get; set; }
        public ICollection<userQuiz> quizDetail { get; set; }
    }
    public class userQuiz
    {
        public Question question { get; set; }
        public evaQuestionDetail detail { get; set; }
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