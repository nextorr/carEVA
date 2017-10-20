using carEVA.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace carEVA.ViewModels
{
    public class externalUserViewModel
    {
        public int userID { get; set; }
        [DisplayName("Nombre completo")]
        [Required]
        public string userFullName { get; set; }
        [DisplayName("Tipo de Documento")]
        [Required]
        public documentTypes documentType { get; set; }
        [DisplayName("Numero de documento")]
        [Required]
        public long identificationNumber { get; set; }
    }
    public class carDefensoresPasswordViewModel
    {
        [Required]
        [Display(Name ="Contraseña")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public evaCarDefensoresAgua userInfo { get; set; }
    }
}