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
        [StringLength(60, MinimumLength = 6, ErrorMessage = "La contraseña debe ser de almenos 6 caracteres")]
        public string password { get; set; }
        [Required]
        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "las contraseñas no coinciden")]
        [StringLength(60, MinimumLength = 6)]
        public string confirmPassword { get; set; }
        public evaCarDefensoresAgua userInfo { get; set; }
    }
}