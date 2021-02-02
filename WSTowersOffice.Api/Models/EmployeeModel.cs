using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSTowersOffice.Api.Models
{
    public class EmployeeModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 11)]
        public string CPF { get; set; }
        [Required]
        [StringLength(3, MinimumLength = 500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public int QuantityFamilyPersons { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(1000, MinimumLength = 4)]
        public string Email { get; set; }

        public FileModel ProfileImage { get; set; }

        public EmployeeModel(Employee employee)
        {
            ID = employee.ID;
            CPF = employee.CPF;
            Name = employee.Name;
            QuantityFamilyPersons = employee.QuantityFamilyPersons;
            Email = employee.Email;
            ProfileImage = new FileModel(employee.File);
        }

        public Employee GetEmployee()
        {
            return new Employee()
            {
                CPF = CPF,
                Name = Name,
                ProfileImage = 2,
                Email = Email,
                QuantityFamilyPersons = QuantityFamilyPersons
            };
        }
    }
}