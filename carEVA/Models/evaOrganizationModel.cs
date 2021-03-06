﻿using System;
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
        public virtual ICollection<evaOrgCourseAreaPermissions> audiencePerCourseAreas { get; set; }
        public virtual ICollection<evaBaseUser> usersInGroup { get; set; }

    }
    //this enumeration defines the standarized access levels
    public enum areaPermission
    {
        deny, //all the users in the area cant access the course.
        read, //can only view the information, but cannot enroll.
        canEnrol, //all the users in the area can enrol in the course.
    }
    //like the course enrollment, this is used to autorize Areas to view certain courses
    public class evaOrgCourseAreaPermissions
    {
        public int evaOrgCourseAreaPermissionsID { get; set; }
        public int evaOrganizationCourseID { get; set; }
        public int evaOrganizationAreaID { get; set; }
        public virtual evaOrganizationCourse evaOrganizationCourse { get; set; }
        public virtual evaOrganizationArea evaOrganizationArea { get; set; }
        //--------------Join table payload-----------------------------
        public areaPermission permissionLevel { get; set; }
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
        public virtual ICollection<evaOrgCourseAreaPermissions> audienceAreas { get; set; }
        //instructor and colaborators of the course
        public int evaInstructorID { get; set; }
        [ForeignKey("evaInstructorID ")]
        public virtual evaInstructor instructor { get; set; }
        public virtual ICollection<evaInstructor> colaborators { get; set; }
        //public virtual ICollection<evaInstructor> assistants { get; set; }
    }
    //*********************************************************************************************
    //comparator interface to allow union operations
    public class evaOrganizationCourseComparer : IEqualityComparer<evaOrganizationCourse>
    {
        public bool Equals(evaOrganizationCourse x, evaOrganizationCourse y)
        {
            //return true if they reference the same object in memory
            if (Object.ReferenceEquals(x, y)) return true;

            //check only ID as two distinct courses cant have the same ID
            //compare also the title just to make sure
            return x != null 
                && y != null 
                && x.evaOrganizationCourseID.Equals(y.evaOrganizationCourseID)
                && x.creationDate.Equals(y.creationDate);
        }

        public int GetHashCode(evaOrganizationCourse obj)
        {
            //get has code of ID 
            int hashOrgCourseID = obj.evaOrganizationCourseID.GetHashCode();
            int hashCreationDate = obj.creationDate.ToString() == null 
                ? 0 : obj.creationDate.ToString().GetHashCode();

            return hashOrgCourseID ^ hashCreationDate;
        }
    }
    //*********************************************************************************************
}