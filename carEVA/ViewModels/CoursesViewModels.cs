using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.ViewModels
{
    /// <summary>
    ///this denormalizes the course entity, as once logger a course only has 
    ///the organization info of the logged user
    /// </summary>
    public class CoursesViewModels
    {
        public Course course { get; set; }
        public evaOrganizationCourse organizationInfo { get; set; }
        public bool isInternal { get { return !organizationInfo.originArea.isExternal; } }

    }
}