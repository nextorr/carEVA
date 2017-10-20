using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class evaOrganization
    {
        public int evaOrganizationID { get; set; }
        [DisplayName("Nombre Entidad")]
        [Required(ErrorMessage = "Debe ingresar un nombre para la Organizacion")]
        [StringLength(450)]
        [Index(IsUnique = true)]
        public string name { get; set; }
        [DisplayName("Nombre corto")]
        public string nameShort { get; set; }
        [DisplayName("Nombre alternativo")]
        public string nameAlternative { get; set; }
        [DisplayName("Nombre en siglas")]
        public string nameAbbreviation { get; set; }
        [DisplayName("Slogan corporativo")]
        public string slogan { get; set; }
        [DisplayName("Dominio")]
        [Required(ErrorMessage = "Debe ingresar un nombre de dominio para la organizacion")]
        [StringLength(450)]
        [Index(IsUnique = true)]
        public string domain { get; set; }
        [DisplayName("Pais oficina principal")]
        public string mainOfficeCountry { get; set; }
        [DisplayName("Ciudad oficina principal")]
        public string mainOfficeCity { get; set; }
        [DisplayName("Direccion")]
        public string address { get; set; }
        public string phone { get; set; }
        //score card of the organization.
        public int totalCatalogCourses { get; set; }
        public int totalRequiredCourses { get; set; }
        //..................navigation properties.....................
        public virtual ICollection<evaOrganizationArea> evaAreas { get; set; }
        public virtual ICollection<evaOrganizationCourse> organizationCourses { get; set; }
        public virtual ICollection<evaBaseUser> users { get; set; }
    }
    //areas for each organization
    public class evaOrganizationArea
    {
        public int evaOrganizationAreaID { get; set; }
        //some organizations have area numbers because the area name can change.
        [DisplayName("Codigo del Area")]
        public string areaCode { get; set; }
        [DisplayName("Nombre del Area")]
        public string name { get; set; }
        [DisplayName("Sigla del Area")]
        public string nameAbreviation { get; set; }
        [DisplayName("Habilitar")]
        public bool isEnabled { get; set; }
        [DisplayName("Es Area Externa")]
        public bool isExternal { get; set; }
        public string externalType { get; set; }
        //......................Navigation Properties..............................
        public int evaOrganizationID { get; set; }
        public virtual evaOrganization organization { get; set; }
        //read this as: courses that has this Origin area. 1course -> 1 areas; 1area -> many courses
        public virtual ICollection<evaOrganizationCourse> organizationCourses { get; set; }
        public virtual ICollection<audiencePerCourse> audiencePerCourseAreas { get; set; }
        public virtual ICollection<evaBaseUser> usersInGroup { get; set; }

    }
    //like the course enrollment, this is used to autorize Areas to view certain courses
    public class audiencePerCourse
    {
        public int audiencePerCourseID { get; set; }
        public int evaOrganizationCourseID { get; set; }
        public int evaOrganizationAreaID { get; set; }
        public evaOrganizationCourse evaOrganizationCourse { get; set; }
        public evaOrganizationArea evaOrganizationArea { get; set; }
    }
    //relates many organizations to many courses with payload
    public class evaOrganizationCourse
    {
        public int evaOrganizationCourseID { get; set; }
        //the payload
        [DisplayName("Fecha de creacion")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime creationDate { get; set; }
        [DisplayName("Es obligatorio")]
        public bool required { get; set; }
        [DisplayName("Fecha Limite de inscripcion")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true, NullDisplayText ="No Aplica")]
        public DateTime? deadline { get; set; }
        //...............navigation properties ......................
        //many to many organization to courses
        public int evaOrganizationID { get; set; }
        public virtual evaOrganization organization { get; set; }
        public int courseID { get; set; }
        public virtual Course course { get; set; }
        [DisplayName("ID Area de Origen")]
        public int originAreaID { get; set; }
        [ForeignKey("originAreaID")]
        public virtual evaOrganizationArea originArea { get; set; }
        public virtual ICollection<audiencePerCourse> audienceAreas { get; set; }
    }
}