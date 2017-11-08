using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class municipio
    {
        public int municipioID { get; set; }
        public string nombre { get; set; }
        //......................Navigation Properties..............................
        //the Municipio has to be associated with only one Organization
        //so each organization can define different zones
        public int evaOrganizationID { get; set; }
        [ForeignKey("evaOrganizationID")]
        public virtual evaOrganization organization { get; set; }
    }
}