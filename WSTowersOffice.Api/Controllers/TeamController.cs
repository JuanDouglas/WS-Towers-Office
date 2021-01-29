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

namespace WSTowersOffice.Api.Controllers
{

    [RoutePrefix("Team")]
    public class TeamController : Controller
    {
        public WSTowersOfficeEntities db => new WSTowersOfficeEntities();
        // GET: Teams
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListTeams()
        {
            Team[] team = await db.Team.Where(wh=>true).ToArrayAsync();

            if (team == null)
            {
                return HttpNotFound();
            }

            return View(team);
        }

        public async Task<ActionResult> ManageTeam(int team_id) {
            Team team = await db.Team.FirstOrDefaultAsync(fs => fs.ID == team_id);

            if (team == null)
            {
                return HttpNotFound();
            }

            return View(team);
        }
        public async Task<ActionResult> Create() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include ="Name,Description")]TeamModel team) 
        {
            if ((await db.Team.FirstOrDefaultAsync(fs=>fs.Name==team.Name))!=null)
            {
                ModelState.AddModelError("Name","This name already use!");
            }

            if (!ModelState.IsValid)
            {
                return View(team);
            }

            return View(team);
        }

        [Route("SetTeamIcon/{team_id}")]
        [HttpPost]
        public async Task<ActionResult> SetTeamIcon(int team_id) {

            Team team = await db.Team.FirstOrDefaultAsync(fs=>fs.ID==team_id);

            if (team==null)
            {
                return HttpNotFound();
            }

            HttpFileCollection fileCollection = System.Web.HttpContext.Current.Request.Files;

            if (fileCollection.Count<1)
            {
                return RedirectToActionPermanent("BadRequest","Errors");
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

                FileModel fileModel = await FilesController.SaveImageAsync(FileType.TeamIcon, image ,fileCollection[0].ContentLength);
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

    }
}