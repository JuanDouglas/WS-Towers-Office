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

        public TeamModel()
        {
        }

        public TeamModel(Team team)
        {
            ID = team.ID;
            Name = team.Name;
            Description = team.Description;
            Icon = new FileModel(team.File);
        }

        public int ID { get; set; }
        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Descrição")]
        [StringLength(1000,MinimumLength = 5)]
        public string Description { get; set; }
        [Display(Name = "Icone")]
       public FileModel Icon { get; set; }
        private int IconID { get; set; }
        internal Team GetTeam()
        {
            IconID = 2;
            return new Team() {
                Name = Name,
                Description = Description,
                Icon = IconID
            };
        }
    }
}