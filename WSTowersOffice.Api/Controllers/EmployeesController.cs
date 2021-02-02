using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WSTowersOffice.Api.Models;
using WSTowersOffice.Api.Models.Enums;

namespace WSTowersOffice.Api.Controllers
{
    public class EmployeesController : Controller
    {
        private WSTowersOfficeEntities db = new WSTowersOfficeEntities();
        public async Task<ActionResult> Index()
        {
            List<Employee> employees = await db.Employee.Where(wh => true).ToListAsync();
            List<EmployeeModel> employeeModels = new List<EmployeeModel>();
            foreach (Employee employee in employees)
            {
                employeeModels.Add(new EmployeeModel(employee));
            }
            return View(employeeModels);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.SetImage = false;
            return View(new EmployeeModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CPF,Name,QuantityFamilyPersons,Email")] EmployeeModel employeeModel)
        {
            if ((await db.Employee.FirstOrDefaultAsync(fs => fs.CPF == employeeModel.CPF)) != null)
            {
                ModelState.AddModelError("CPF", "Already exist one Employee using this CPF!");
            }

            if ((await db.Employee.FirstOrDefaultAsync(fs => fs.CPF == employeeModel.CPF)) != null)
            {
                ModelState.AddModelError("Email", "Already exist one Employee using this Email!");
            }

            if (!ModelState.IsValid)
            {
                return View(employeeModel);
            }

            Employee employee = employeeModel.GetEmployee();
            db.Employee.Add(employee);
            await db.SaveChangesAsync();

            employee = await db.Employee.FirstOrDefaultAsync(fs=>fs.CPF==employeeModel.CPF);
            ViewBag.Action = $"SetProfileImage/{employee.ID}";
            ViewBag.Controller= "Employees";
            ViewBag.SetImage=true;
            return View(new EmployeeModel(employee));
        }

        [HttpPost]
        public async Task<ActionResult> SetProfileImage(int id)
        {
            Employee employee = await db.Employee.FirstOrDefaultAsync(fs => fs.ID == id);

            if (employee == null)
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

                FileModel fileModel = await FilesController.SaveImageAsync(FileType.EmployeeProfileImage, image, fileCollection[0].ContentLength);
                employee.ProfileImage = fileModel.ID;

                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToActionPermanent("InternalError", "Errors");
            }

            return RedirectToAction("", "");
        }
    }
}
