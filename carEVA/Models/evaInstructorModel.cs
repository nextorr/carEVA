using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;



namespace carEVA.Models
{
    public class evaInstructor : evaBaseUser
    {
        public string altEmail { get; set; }
        //use this for contact information.
        public string mobileNumber { get; set; }
        //courses the instructor own
        public virtual ICollection<Course> coursesOwned { get; set; }
        public virtual ICollection<evaOrganizationCourse> instructorOf { get; set; }
        [InverseProperty("colaborators")]
        public virtual ICollection<evaOrganizationCourse> colaboratorOf { get; set; }
        //[InverseProperty("assistants")]
        //public virtual ICollection<evaOrganizationCourse> assistantOf { get; set; }

        public override string getIndexViewName
        {
            get
            {
                return "_userInstructorList";
            }
        }
        public override string getCreateActionName
        {
            get
            {
                throw new NotImplementedException(); 
            }
        }
        public override string getEditViewName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}