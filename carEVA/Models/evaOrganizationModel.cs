using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class evaOrganization
    {
        public int evaOrganizationID { get; set; }
        public string name { get; set; }
        public string domain { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public virtual ICollection<evaOrganizationArea> evaAreas { get; set; }
    }
    //areas for each organization
    public class evaOrganizationArea
    {
        public int evaOrganizationAreaID { get; set; }
        public int evaOrganizationID { get; set; }
        //some organizations have area numbers because the area name can change.
        public int areaNumber { get; set; }
        public string name { get; set; }
        public virtual evaOrganization organization { get; set; }
    }
    //relates the organizations and its courses with payload
    public class evaOrganizationCourse
    {
        public int evaOrganizationCourseID { get; set; }
        public int evaOrganizationID { get; set; }
        public int courseID { get; set; }
        public virtual evaOrganization organization { get; set; }
        public virtual Course course { get; set; }
        //the payload
        public int evaAreaOriginID { get; set; }
        public virtual evaOrganizationArea originArea { get; set; }
        public virtual ICollection<evaOrganizationArea> audienceArea { get; set; }
        public DateTime creationDate { get; set; }
        public bool required { get; set; }
        public DateTime deadline { get; set; }
    }
}