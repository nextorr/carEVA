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
        //TODO: implement area CAR, see if its better by name or ID
        //the user can be associated with only one Organization
        public virtual ICollection<Course> courses { get; set; }
        public override string getIndexViewName
        {
            get
            {
                return "_userInstructorList";
            }
        }
        public override string getCreateViewName
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