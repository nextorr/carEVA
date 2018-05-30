using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace carEVA.Models
{
    //take special care of the enum values
    public enum documentTypes
    {
        //tarjeta de identidad
        TI,
        //cedula de ciudadania
        CC
    }
    
    
    public class externalUserContainer
    {
        public string className { get; set; }
        public Type type { get; set; }
    }
    public static class externalUsers
    {
        public static List<externalUserContainer> definitions
        {
            get
            {
                return new List<externalUserContainer>() {
                    new externalUserContainer() {className= "evaCarDefensoresAgua", type = typeof (evaCarDefensoresAgua) }
                };
            }
        }
        public static evaBaseUser createInstance(string externalType)
        {
            switch (externalType)
            {
                case "evaCarDefensoresAgua":
                    return new evaCarDefensoresAgua();
                default:
                    throw new ArgumentException("Tried to instantiate a not registered type");
            }
        }
    }
    //Remember: an external user is defined by its pertenence to an external area
    //the idea here is to define models to capture aditional info depending on the 
    //needs of each external course
    public class evaCarDefensoresAgua : evaBaseUser
    {
        [DisplayName("Institucion educativa")]
        public string institucionEducativa { get; set; }
        [DisplayName("Tipo de documento")]
        public documentTypes tipoDocumento { get; set; }
        [DisplayName("Numero de documento")]
        public long numeroDocumento { get; set; }
        [Range(13,18)]
        public int? edad { get; set; }
        public string municipio { get; set; }
        [DisplayName("Grado de estudio")]
        [Range(1, 11)]
        public string gradoEstudio { get; set; }
        public override string getIndexViewName
        {
            get
            {
                return "_carDefensoresAguaUserList";
            }
        }
        public override string getCreateActionName
        {
            get
            {
                return "CreateEvaCarDefensoresAgua";
            }
        }
        public override string getEditViewName
        {
            get
            {
                return "evaCarDefensoresAguaEdit";
            }
        }
        public override string registerUserControllerName()
        {
            return "registerEvaCarDefensoresdelAgua";
        }
    }
}