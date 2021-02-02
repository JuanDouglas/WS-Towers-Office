using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSTowersOffice.Api.Models
{
    public class TeamModel
    {
        private WSTowersOfficeEntities db = new WSTowersOfficeEntities();
        public int ID { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [StringLength(1000,MinimumLength = 5)]
        public string Description { get; set; }
        private int Icon { get; set; }
        internal Team GetTeam()
        {
            Icon = 2;
            return new Team() {
                Name = Name,
                Description = Description,
                Icon = Icon
            };
        }
    }
}