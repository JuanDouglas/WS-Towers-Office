using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTowersOffice.Api.Models
{
    public class TeamEmployeeModel
    {
        public TeamEmployeeModel(Team_Employee item)
        {
            Employee = new EmployeeModel(item.Employee1);
            Team = new TeamModel(item.Team1);
            AddDate = item.AddDate.AddHours(-3);
        }

        public TeamEmployeeModel()
        {
        }

        public EmployeeModel Employee { get; set; }
        public TeamModel Team { get; set; }
        public DateTime AddDate { get; set; }
    }
}