using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
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
        public int edad { get; set; }
        public string municipio { get; set; }
        [DisplayName("Grado de estudio")]
        public string gradoEstudio { get; set; }
        public override string getIndexViewName
        {
            get
            {
                return "_carDefensoresAguaUserList";
            }
        }
        public override string getCreateViewName
        {
            get
            {
                return "evaCarDefensoresAguaCreate";
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