using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class evaDbInit2 : System.Data.Entity.DropCreateDatabaseIfModelChanges<carEVAContext>
    {
        protected override void Seed(carEVAContext context)
        {
            var courses = new List<Course>
            {
                new Course {title = "CURSO MANEJO SIDCAR", description = "Aprenda el funcionamiento del sistema documental de la CAR SIDCAR" },
                new Course {title = "CURSO MANEJO SAE", description = "Aprenda el funcionamiento del sistema de administracion de expedientes de la CAR SAE" },
            };

            courses.ForEach(s => context.Courses.Add(s));
            context.SaveChanges();
            var chapters = new List<Chapter>
            {
                new Chapter {CourseID= 1, title = "Manejo basico del sistema", index=1 },
                new Chapter {CourseID= 1, title = "Manejo de la documentacion digital", index=2 },
            };
            chapters.ForEach(s => context.Chapters.Add(s));
            context.SaveChanges();
            var lessons = new List<Lesson>
            {
                new Lesson {ChapterID=1, title = "Informacion basica", description="Aprenda a configurar su informacion personal dentro del sistema" , videoURL = "wait and see wich URL do we need and if it depends on android or IOS"  },
                new Lesson {ChapterID=1, title = "Cambio de contraseña", description="Pasos necesarios para cambiar su contraseña, o re establecer una nueva en caso de olvido" , videoURL = "wait and see wich URL do we need and if it depends on android or IOS"  },
                new Lesson {ChapterID=2, title = "Crear memorandos digitales", description="Aprenda todo el funcionamiento de los memorandos digitales en SIDCAR" , videoURL = "wait and see wich URL do we need and if it depends on android or IOS"  },
                new Lesson {ChapterID=2, title = "Copiar contenido desde word", description="SIDCAR incluye un editor de texto, si tiene la informacion en Word siga estos pasos para pegar esta informacion en el editor de SIDCAR." , videoURL = "wait and see wich URL do we need and if it depends on android or IOS"  },
                new Lesson {ChapterID=2, title = "Firmas digitales", description="Como inculir una firma digital dentro de su documento SIDCAR." , videoURL = "wait and see wich URL do we need and if it depends on android or IOS"  },
            };
            lessons.ForEach(s => context.Lessons.Add(s));
            context.SaveChanges();
            var questions = new List<Question>
            {
                new Question {LessonID = 1, statement = "Cual es la forma mas rapida de crear un memorando?", evaType="single" },
                new Question {LessonID = 1, statement = "cual opcion me permite realizar un cambio de contraseña?", evaType="single" },
            };
            questions.ForEach(s => context.Questions.Add(s));
            context.SaveChanges();
            var answers = new List<Answer>
            {
                new Answer {QuestionID = 1, text="ingresando la informacion directamente en SIDCAR", isCorrect=true },
                new Answer {QuestionID = 1, text="Crear el text en word y luego pegarlo en SIDCAR", isCorrect=false },
                new Answer {QuestionID = 1, text="Crear el texto el word y usar la opcion de pegado", isCorrect=false },
                new Answer {QuestionID = 2, text="en el inicio de sesion del sistema", isCorrect=false },
                new Answer {QuestionID = 2, text="En la opcion crear memorando", isCorrect=false },
                new Answer {QuestionID = 2, text="Entrando a mi perfir de usuario", isCorrect=true },
            };
            answers.ForEach(s => context.Answers.Add(s));
            context.SaveChanges();
        }
    }
}