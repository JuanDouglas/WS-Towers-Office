using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSTowersOffice.Execution.Api.Models
{
    public class EmployeeModel
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "CPF")]
        [StringLength(15, MinimumLength = 11)]
        public string CPF { get; set; }
        [Required]
        [Display(Name = "Nome")]
        [StringLength(500, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Pessoas na família")]
        public int QuantityFamilyPersons { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        [StringLength(1000, MinimumLength = 4)]
        public string Email { get; set; }
        [Display(Name = "Imagem de perfil")]
        public FileModel ProfileImage { get; set; }
    }
}