namespace carEVA.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<carEVA.Models.carEVAContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(carEVA.Models.carEVAContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //seed the permission table with default values for initialization.
            //WARNIGN: delete this after the initial seed, to prevent overwriting permissions changes
            //List<evaOrgCourseAreaPermissions> permissions = new List<evaOrgCourseAreaPermissions>();
            ////get all the internal organization courses and add access to all the internal areas 
            ////unrestricted access to all the courses!
            //List<evaOrganizationCourse> orgCourses = context.evaOrganizationAreas
            //    .Include(o=>o.organizationCourses)
            //    .Where(o => o.isExternal == false)
            //    .SelectMany(o=>o.organizationCourses).ToList();
            //List<evaOrganizationCourse> ExternalCourses = context.evaOrganizationAreas
            //    .Include(o => o.organizationCourses)
            //    .Where(o => o.isExternal)
            //    .SelectMany(o => o.organizationCourses).ToList();
            //List<evaOrganizationArea> orgAreas = context.evaOrganizationAreas
            //    .Where(o => o.isExternal == false).ToList();
            //List<evaOrganizationArea> externalAreas = context.evaOrganizationAreas
            //    .Where(o => o.isExternal).ToList();
            //foreach (evaOrganizationCourse orgCourse in orgCourses)
            //{
            //    foreach (evaOrganizationArea orgArea in orgAreas)
            //    {
            //        //internal areas has access to the internal courses
            //        permissions.Add(new evaOrgCourseAreaPermissions {
            //            evaOrganizationCourseID=orgCourse.evaOrganizationCourseID,
            //            evaOrganizationAreaID=orgArea.evaOrganizationAreaID,
            //            permissionLevel= areaPermission.canEnrol
            //        });
            //    }
            //}
            //foreach (evaOrganizationCourse orgCourse in ExternalCourses)
            //{
            //    foreach (evaOrganizationArea orgArea in externalAreas)
            //    {
            //        //external areas has permission to the external courses
            //        permissions.Add(new evaOrgCourseAreaPermissions
            //        {
            //            evaOrganizationCourseID = orgCourse.evaOrganizationCourseID,
            //            evaOrganizationAreaID = orgArea.evaOrganizationAreaID,
            //            permissionLevel = areaPermission.canEnrol
            //        });
            //    }
            //}
            //context.evaOrgCourseAreaPermissions.AddRange(permissions);
            //context.SaveChanges();


            //we no longer need this initialization method. 
            //so it does not interfere with existing data.
            //var evaOrganizations = new List<evaOrganization>
            //{
            //    new evaOrganization {name = "Corporacion Autonoma Regional de Cundinamarca (pruebas)", domain="car.gov.co", address= "carrera 7 # 36 - 45", phone= "3209000" }
            //};
            //evaOrganizations.ForEach(o => context.evaOrganizations.AddOrUpdate(p => p.name, o));
            //context.SaveChanges();
            ////this is required to populate the list of supported answer types
            //var evaTypes = new List<evaType>
            //{
            //    new evaType {answerType = "single" }
            //};
            //evaTypes.ForEach(s => context.evaTypes.AddOrUpdate(p => p.answerType, s));
            //context.SaveChanges();

            //var courses = new List<Course>
            //{
            //    new Course {title = "CURSO MANEJO SIDCAR (pruebas)", description = "Aprenda el funcionamiento del sistema documental de la CAR SIDCAR", evaImageID = 1 },
            //    new Course {title = "CURSO MANEJO SAE (pruebas)", description = "Aprenda el funcionamiento del sistema de administracion de expedientes de la CAR SAE", evaImageID=1 },
            //};
            ////this checks for unique titles, and fails if there are more than one course with the same title
            //courses.ForEach(s => context.Courses.AddOrUpdate(p =>p.title, s));
            //context.SaveChanges();

            //var chapters = new List<Chapter>
            //{
            //    new Chapter
            //    {
            //        CourseID = courses.Single(c => c.title == "CURSO MANEJO SIDCAR (pruebas)").CourseID,
            //        title = "Manejo basico del sistema (pruebas)", index=1
            //    },
            //    new Chapter
            //    {
            //        CourseID = courses.Single(c => c.title == "CURSO MANEJO SIDCAR (pruebas)").CourseID,
            //        title = "Manejo de la documentacion digital (pruebas)", index=2
            //    },
            //};
            //chapters.ForEach(s => context.Chapters.AddOrUpdate(p => p.title, s));
            //context.SaveChanges();

            //var lessons = new List<Lesson>
            //{
            //    new Lesson
            //    {
            //        ChapterID =chapters.Single(c => c.title == "Manejo basico del sistema (pruebas)").ChapterID,
            //        title = "Informacion basica (pruebas)",
            //        description ="Aprenda a configurar su informacion personal dentro del sistema" ,
            //        videoURL = "wait and see wich URL do we need and if it depends on android or IOS"
            //    },
            //    new Lesson
            //    {
            //        ChapterID =chapters.Single(c => c.title == "Manejo basico del sistema (pruebas)").ChapterID,
            //        title = "Cambio de contrase�a (pruebas)",
            //        description ="Pasos necesarios para cambiar su contrase�a, o re establecer una nueva en caso de olvido" ,
            //        videoURL = "wait and see wich URL do we need and if it depends on android or IOS"
            //    },
            //    new Lesson
            //    {
            //        ChapterID =chapters.Single(c => c.title == "Manejo de la documentacion digital (pruebas)").ChapterID,
            //        title = "Crear memorandos digitales (pruebas)",
            //        description ="Aprenda todo el funcionamiento de los memorandos digitales en SIDCAR" ,
            //        videoURL = "wait and see wich URL do we need and if it depends on android or IOS"
            //    },
            //    new Lesson
            //    {
            //        ChapterID =chapters.Single(c => c.title == "Manejo de la documentacion digital (pruebas)").ChapterID,
            //        title = "Copiar contenido desde word (pruebas)",
            //        description ="SIDCAR incluye un editor de texto, si tiene la informacion en Word siga estos pasos para pegar esta informacion en el editor de SIDCAR." ,
            //        videoURL = "wait and see wich URL do we need and if it depends on android or IOS"
            //    },
            //    new Lesson
            //    {
            //        ChapterID =chapters.Single(c => c.title == "Manejo de la documentacion digital (pruebas)").ChapterID,
            //        title = "Firmas digitales (pruebas)",
            //        description ="Como inculir una firma digital dentro de su documento SIDCAR." ,
            //        videoURL = "wait and see wich URL do we need and if it depends on android or IOS"
            //    },
            //};
            //lessons.ForEach(s => context.Lessons.AddOrUpdate(l => l.title, s));
            //context.SaveChanges();

            //var questions = new List<Question>
            //{
            //    new Question
            //    {
            //        LessonID = lessons.Single(l => l.title == "Informacion basica (pruebas)").LessonID,
            //        statement = "Cual es la forma mas rapida de crear un memorando? (pruebas)",
            //        evaType ="single"
            //    },
            //    new Question
            //    {
            //        LessonID = lessons.Single(l => l.title == "Informacion basica (pruebas)").LessonID,
            //        statement = "cual opcion me permite realizar un cambio de contrase�a? (pruebas)",
            //        evaType ="single"
            //    },
            //};
            //questions.ForEach(s => context.Questions.AddOrUpdate(q => q.statement, s));
            //context.SaveChanges();

            //var answers = new List<Answer>
            //{
            //    new Answer
            //    {
            //        QuestionID = questions.Single(q=> q.statement == "Cual es la forma mas rapida de crear un memorando? (pruebas)").QuestionID,
            //        text ="ingresando la informacion directamente en SIDCAR (pruebas)",
            //        isCorrect =true
            //    },
            //    new Answer
            //    {
            //        QuestionID = questions.Single(q=> q.statement == "Cual es la forma mas rapida de crear un memorando? (pruebas)").QuestionID,
            //        text ="Crear el text en word y luego pegarlo en SIDCAR (pruebas)",
            //        isCorrect =false
            //    },
            //    new Answer
            //    {
            //        QuestionID = questions.Single(q=> q.statement == "Cual es la forma mas rapida de crear un memorando? (pruebas)").QuestionID,
            //        text ="Crear el texto el word y usar la opcion de pegado (pruebas)",
            //        isCorrect =false
            //    },
            //    new Answer
            //    {
            //        QuestionID = questions.Single(q=> q.statement == "cual opcion me permite realizar un cambio de contrase�a? (pruebas)").QuestionID,
            //        text ="en el inicio de sesion del sistema (pruebas)",
            //        isCorrect =false
            //    },
            //    new Answer
            //    {
            //        QuestionID = questions.Single(q=> q.statement == "cual opcion me permite realizar un cambio de contrase�a? (pruebas)").QuestionID,
            //        text ="En la opcion crear memorando (pruebas)",
            //        isCorrect =false
            //    },
            //    new Answer
            //    {
            //        QuestionID = questions.Single(q=> q.statement == "cual opcion me permite realizar un cambio de contrase�a? (pruebas)").QuestionID,
            //        text ="Entrando a mi perfir de usuario (pruebas)",
            //        isCorrect =true
            //    },
            //};
            //answers.ForEach(s => context.Answers.AddOrUpdate(a => a.text, s));
            //context.SaveChanges();
        }
    }
}
