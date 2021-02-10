using System.Web.Mvc;
using WSTowersOffice.Api.Models;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using WSTowersOffice.Api.Models.Enums;
using System.Drawing;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using WSTowersOffice.Api.Properties;
using System.Configuration;
using ShowProducts.API.Controllers;
using System.Data.SqlClient;

namespace WSTowersOffice.Api.Controllers
{
    [RoutePrefix("Teams")]
    public class TeamsController : Controller
    {
        public WSTowersOfficeEntities db => new WSTowersOfficeEntities();
        // GET: Teams
        public async Task<ActionResult> Index()
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            List<Team> teams = await db.Team.Where(wh => true).ToListAsync();

            if (teams == null)
            {
                return HttpNotFound();
            }

            List<TeamModel> teamsModel = new List<TeamModel>();
            foreach (Team item in teams)
            {
                teamsModel.Add(new TeamModel(item));
            }
            return View(teamsModel);
        }

        [HttpGet]
        [Route("Management/{team_name}")]
        public async Task<ActionResult> Management(string team_name, TeamManagementOrdeBy? order_by, string query, int? page)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            ViewBag.ContainEmployee = false;
            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);

            if (team == null)
            {
                return HttpNotFound();
            }

            Employee[] employees = await db.Employee.Where(wh => true).ToArrayAsync();
            List<SelectListItem> employeesList = new List<SelectListItem>();
            foreach (Employee employee in employees)
            {
                if ((await db.Team_Employee.FirstOrDefaultAsync(fs => fs.Team == team.ID && fs.Employee == employee.ID)) == null)
                {
                    employeesList.Add(new SelectListItem()
                    {
                        Text = $"{employee.Name.Split(' ')[0]}, {employee.CPF}",
                        Value = employee.ID.ToString()
                    });
                }
            }

            ViewBag.Employees = employeesList;
            Team_Role[] teamRoles = await db.Team_Role.Where(wh => wh.Team == team.ID).ToArrayAsync();
            List<SelectListItem> rolesList = new List<SelectListItem>();
            foreach (Team_Role role in teamRoles)
            {
                rolesList.Add(new SelectListItem()
                {
                    Text = $"{role.Role1.Name}",
                    Value = role.Role1.Name
                });

            }

            ViewBag.TeamRoles = rolesList;
            TeamEmployeeModel[] teamEmployees = new TeamEmployeeModel[0];
            IQueryable<Team_Employee> teamEmployeesQueryable = db.Team_Employee.Where(wh => wh.Team == team.ID);
            if (query != null)
            {
                if (query != "")
                {
                    teamEmployeesQueryable = teamEmployeesQueryable.Where(wh => wh.Employee1.Name.Contains(query));
                }
            }
            if (!order_by.HasValue)
            {
                order_by = TeamManagementOrdeBy.DecreasingAddDate;
            }
            teamEmployeesQueryable = TeamOrdeBy(order_by.Value, teamEmployeesQueryable).AsQueryable();
            if (!page.HasValue)
            {
                page = 0;
            }
            teamEmployees = GetTeamEmployeesPage(teamEmployeesQueryable, page.Value);
            ViewBag.TeamEmployees = teamEmployees;

            return View(new TeamModel(team));
        }

        private TeamEmployeeModel[] GetTeamEmployeesPage(IQueryable<Team_Employee> teamEmployeesQueryable, int page)
        {
            List<TeamEmployeeModel> teamEmployees = new List<TeamEmployeeModel>();
            for (int i = (10 * (page + 1)); i < teamEmployeesQueryable.Count() && i < (10 * (page + 1)); i++)
            {
                teamEmployees.Add(new TeamEmployeeModel());
            }
            return teamEmployees.ToArray();
        }

        private IOrderedQueryable<Team_Employee> TeamOrdeBy(TeamManagementOrdeBy order, IQueryable<Team_Employee> employees)
        {
            IOrderedQueryable<Team_Employee> team_employees = null;
            switch (order)
            {
                case TeamManagementOrdeBy.DecreasingAddDate:
                    team_employees = employees.OrderByDescending(employee => employee.AddDate);
                    break;
                case TeamManagementOrdeBy.AscendingAddDate:
                    team_employees = employees.OrderBy(employee => employee.AddDate);
                    break;
                default:
                    break;
            }
            return team_employees;
        }

        public async Task<ActionResult> Create()
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string team_name)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);

            if (team == null)
            {
                return HttpNotFound();
            }

            return View(new TeamModel(team));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(string team_name)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);
            if (team == null)
            {
                return HttpNotFound();
            }

            foreach (var item in team.Team_Role)
            {
                db.Team_Role.Remove(item);
            }

            foreach (var item in team.Team_Employee)
            {
                db.Team_Employee.Remove(item);
            }

            await db.SaveChangesAsync();
            using (SqlConnection connection = new SqlConnection(WebApiConfig.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand($"DELETE [Team] WHERE [ID] = {team.ID}", connection);


                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description")] TeamModel team)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            if ((await db.Team.FirstOrDefaultAsync(fs => fs.Name == team.Name)) != null)
            {
                ModelState.AddModelError("Name", "This name already use!");
            }
            if (team.Name.Contains('\"'))
            {
                ModelState.AddModelError("Name","Hey don't use \" \' \" in team name.");
            }
            if (!ModelState.IsValid)
            {
                return View(team);
            }
            Team teamModel = team.GetTeam();
            //db.Team.Add(teamModel);

            //await db.SaveChangesAsync();

            using (SqlConnection connection = new SqlConnection(WebApiConfig.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [Team] ([Name],[Description],[Icon]) VALUES ('{team.Name}','{team.Description}',2);", connection);


                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }

            return RedirectToAction($"Management", "Teams", new { team_name = team.Name });
        }
        [HttpPost]
        [Route("Management/AddIcon")]
        public async Task<ActionResult> AddIcon(int id)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            Team team = await db.Team.FindAsync(id);

            if (team == null)
            {
                return HttpNotFound();
            }

            HttpFileCollection fileCollection = System.Web.HttpContext.Current.Request.Files;

            if (fileCollection.Count < 1)
            {
                return RedirectToActionPermanent("BadRequest", "Errors");
            }

            if (fileCollection[0] != null)
            {
                return RedirectToActionPermanent("BadRequest", "Errors");
            }

            try
            {
                Image image = null;

                try
                {
                    image = Image.FromStream(fileCollection[0].InputStream);
                }
                catch (Exception e)
                {
                    return Content(JsonConvert.SerializeObject(e));
                }

                FileModel fileModel = await FilesController.SaveImageAsync(FileType.TeamIcon, image, fileCollection[0].ContentLength);
                team.Icon = fileModel.ID;

                db.Entry(team).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToActionPermanent("InternalError", "Errors");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(string team_name, string role_name, string role_description)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            RoleModel role = new RoleModel() { Name = role_name, Description = role_description };

            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);
            if (team == null)
            {
                return HttpNotFound();
            }

            bool exist = (await db.Role.FirstOrDefaultAsync(fs => fs.Name == role.Name)) != null;

            if (!exist)
            {
                using (SqlConnection connection = new SqlConnection(WebApiConfig.ConnectionString))
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [Role] ([Name],[Description],[Icon]) VALUES ('{role.Name}','{role.Description}',2);", connection);

                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
                await db.SaveChangesAsync();
            }

            Role roleModel = await db.Role.FirstOrDefaultAsync(fs => fs.Name == role.Name);

            using (SqlConnection connection = new SqlConnection(WebApiConfig.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [Team_Role] ([Team],[Role]) VALUES ('{team.ID}','{roleModel.ID}');", connection);

                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }

            return Redirect(Url.Action("Management", "Teams", new { team_name }));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmployee(int employee_id, string team_name, string role_name, string post)
        {
            var loginResult = await LoginController.ValidLoginAsync();

            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }

            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);
            if (team == null)
            {
                return HttpNotFound();
            }

            Employee employee = await db.Employee.FirstOrDefaultAsync(fs => fs.ID == employee_id);
            if (team == null)
            {
                return HttpNotFound();
            }

            Team_Role teamRole = await db.Team_Role.FirstOrDefaultAsync(fs => fs.Role1.Name == role_name && fs.Team1.ID == team.ID);
            if (teamRole == null)
            {
                return HttpNotFound();
            }


            using (SqlConnection connection = new SqlConnection(WebApiConfig.ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [Team_Employee] ([Team],[Employee],[Role],[AddDate]) VALUES ('{team.ID}','{employee.ID}','{teamRole.ID}','{DateTime.UtcNow}');", connection);

                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return Redirect(post);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}