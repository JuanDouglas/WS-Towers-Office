using System.ComponentModel.DataAnnotations;

namespace WSTowersOffice.Api.Models
{
    public class RoleModel
    {
        [Required]
        [StringLength(2)]
        public string Name { get; set; }
        [StringLength(0)]
        public string Description { get; set; }

        internal Role GetRole()
        {
            return new Role() { Name = Name, Description = Description, Icon = 1 };
        }
    }
}