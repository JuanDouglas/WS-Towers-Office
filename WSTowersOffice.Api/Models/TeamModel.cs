using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSTowersOffice.Api.Models
{
    public class TeamModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [StringLength(1000,MinimumLength = 5)]
        public string Description { get; set; }
    }
}