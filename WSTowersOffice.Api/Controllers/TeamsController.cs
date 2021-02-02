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
using System.Data.SqlClient;
using WSTowersOffice.Api.Properties;
using System.Configuration;

namespace WSTowersOffice.Api.Controllers
{
    public class TeamsController : Controller
    {
        public WSTowersOfficeEntities db => new WSTowersOfficeEntities();
        // GET: Teams
        public async Task<ActionResult> Index()
        {
            List<Team> teams = await db.Team.Where(wh => true).ToListAsync();

            if (teams == null)
            {
                return HttpNotFound();
            }

            return View(teams);
        }

        [HttpGet]
        [Route("Management/{team_name}")]
        public async Task<ActionResult> Management(string team_name)
        {
            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);

            if (team == null)
            {
                return HttpNotFound();
            }

            Employee[] employees = await db.Employee.Where(wh => true).ToArrayAsync();
            List<SelectListItem> employeesList = new List<SelectListItem>();
            foreach (Employee employee in employees)
            {
                employeesList.Add(new SelectListItem()
                {
                    Text = $"{employee.Name.Split(' ')[0]}, {employee.CPF}",
                    Value = employee.ID.ToString()
                });
            }

            ViewBag.Employees = employeesList;

            Team_Role[] roles = await db.Team_Role.Where(wh => wh.Team == team.ID).ToArrayAsync();
            List<SelectListItem> rolesList = new List<SelectListItem>();
            foreach (Team_Role role in roles)
            {
                employeesList.Add(new SelectListItem()
                {
                    Text = $"{role.Role1.Name}",
                    Value = role.Role1.Name
                });
            }
            ViewBag.Roles = rolesList;


            return View(team);
        }
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description")] TeamModel team)
        {
            if ((await db.Team.FirstOrDefaultAsync(fs => fs.Name == team.Name)) != null)
            {
                ModelState.AddModelError("Name", "This name already use!");
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

            return RedirectToAction($"Management", "Teams",new { team_name=team.Name});
        }

        [Route("Management/SetIcon")]
        [HttpPost]
        public async Task<ActionResult> SetIcon(string team_name)
        {

            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.Name == team_name);

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
        [Route("Management/AddEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmployee(int employee_id, string team_name, string role_name, string post)
        {
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

            db.Team_Employee.Add(new Team_Employee()
            {
                Team = team.ID,
                Role = teamRole.ID,
                Employee = employee_id,
                AddDate = DateTime.Now
            });
            await db.SaveChangesAsync();
            return Redirect(post);
        }
    }
}