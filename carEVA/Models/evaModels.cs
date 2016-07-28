﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    //*********************************************************************************************
    //**********COURSE MODEL DEFINITION*************
    public class Course
    {
        public int CourseID { get; set; }
        [DisplayName("Curso")]
        public string title { get; set; }
        [DisplayName("Descripcion")]
        public string description { get; set; }
        //TODO: include de AREA and target audience
        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<evaFile> Files { get; set; }
    }
    public class Chapter
    {
        public int ChapterID { get; set; }
        [DisplayName("Capitulo")]
        public string title { get; set; }
        [DisplayName("Orden")]
        public int index { get; set; }
        public int CourseID { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Lesson> lessons { get; set; }
        public virtual ICollection<evaFile> Files { get; set; }
    }
    public class Lesson
    {
        public int LessonID { get; set; }
        [DisplayName("Leccion")]
        public string title { get; set; }
        [DisplayName("Descripcion")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }
        //the lesson conten as 1/04/2016 is a video
        [DataType(DataType.MultilineText)]
        public string videoURL { get; set; }
        public string videoName { get; set; }
        public string videoStorageName { get; set; }
        public int ChapterID { get; set; }
        public virtual Chapter Chapter { get; set; }
        public virtual ICollection<Question> questions { get; set; }
        public virtual ICollection<evaFile> Files { get; set; }
    }
    public class Question
    {
        public int QuestionID { get; set; }
        [DisplayName("Enunciado")]
        [DataType(DataType.MultilineText)]
        public string statement { get; set; }
        [DisplayName("Tipo de Pregunta")]
        public string evaType { get; set; }
        public int LessonID { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<Answer> answerOptions { get; set; }
    }

    public class Answer
    {
        public int AnswerID { get; set; }
        [DisplayName("Texto")]
        [DataType(DataType.MultilineText)]
        public string text { get; set; }
        [DisplayName("Correcta")]
        public bool isCorrect { get; set; }
        public int QuestionID { get; set; }
        public virtual Question Question { get; set; }
    }
    //**********END OFCOURSE MODEL DEFINITION*************
    //*********************************************************************************************

    //*********************************************************************************************
    //*********STUDENT AND GRADING MODEL DEFINITION***********
    public class Student
    {
        public int StudentID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime creationDate { get; set; }
        //make a connection to the enrollment payload join
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }

    public class Enrollment
    {
        public int enrollmentID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public decimal percentComplete { get; set; }
        public int? grade { get; set; }
        //add a connection to the lesson info
        public virtual ICollection<LessonDetail> lessonDetails { get; set; }
        //foreing keys
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }

    public enum questionState
    {
        correct,
        incorrect,
        incomplete
    }
    public class LessonDetail
    {
       //fully de-normalized scorecard
        public int LessonDetailID { get; set; }
        public int ChapterID { get; set; }
        public int LessonID { get; set; }
        public int QuestionID { get; set; }
        public questionState questionGrade  { get; set; }
        public int timesGraded { get; set; }
        public int timesIncorrect { get; set; }
        public int score { get; set; }
    }
     

    //*********END OF STUDENT AND GRADING MODEL DEFINITION**********
    //*********************************************************************************************

    public class evaType
    {
        public int evaTypeID { get; set; }
        public string answerType { get; set; }
    }

}