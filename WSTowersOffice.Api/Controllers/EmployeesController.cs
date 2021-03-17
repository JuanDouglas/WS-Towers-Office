using Newtonsoft.Json;
using ShowProducts.API.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WSTowersOffice.Api.Models;
using WSTowersOffice.Api.Models.Enums;

namespace WSTowersOffice.Api.Controllers
{
    [RoutePrefix("Employees")]
    public class EmployeesController : Controller
    {
        private readonly WSTowersOfficeEntities db = new WSTowersOfficeEntities();
        public async Task<ActionResult> List(int? employee_id, int? page)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            ViewBag.ContainEmployee = false;
            List<Employee> employees = await db.Employee.Where(wh => true).ToListAsync();
            List<EmployeeModel> employeeModels = new List<EmployeeModel>();
            foreach (Employee employee in employees)
            {
                employeeModels.Add(new EmployeeModel(employee));
            }

            Employee employeeDB = await db.Employee.FirstOrDefaultAsync(fs => fs.ID == employee_id);
            if (employeeDB != null)
            {
                EmployeeModel employeeModel = new EmployeeModel(employeeDB);
                ViewBag.Employee = employeeModel;
                ViewBag.ContainEmployee = true;
                ViewBag.ModalID = "#showEmployeeModal";
            }
            return View(employeeModels);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            ViewBag.SetImage = false;
            return View(new EmployeeModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CPF,Name,QuantityFamilyPersons,Email")] EmployeeModel employeeModel)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            if ((await db.Employee.FirstOrDefaultAsync(fs => fs.CPF == employeeModel.CPF)) != null)
            {
                ModelState.AddModelError("CPF", "Already exist one Employee using this CPF!");
            }

            if ((await db.Employee.FirstOrDefaultAsync(fs => fs.Email == employeeModel.Email)) != null)
            {
                ModelState.AddModelError("Email", "Already exist one Employee using this Email!");
            }

            ViewBag.SetImage = false;
            if (!ModelState.IsValid)
            {
                return View(employeeModel);
            }

            Employee employee = employeeModel.GetEmployee();
            employee.AppActive = false;
            employee.Password = "12345678";
            db.Employee.Add(employee);
            await db.SaveChangesAsync();

            employee = await db.Employee.FirstOrDefaultAsync(fs => fs.CPF == employeeModel.CPF);
            ViewBag.Action = $"SetProfileImage/{employee.ID}";
            ViewBag.Controller = "Employees";
            ViewBag.ModalID = "#addImageModal";
            ViewBag.SetImage = true;
            return View(new EmployeeModel(employee));
        }

        [HttpPost]
        public async Task<ActionResult> SetProfileImage(int id)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
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

            if (fileCollection[0] == null)
            {
                return RedirectToActionPermanent("BadRequest", "Errors");
            }

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


            return RedirectToAction("List", "Employees");
        }
        public async Task<ActionResult> Delete(int? id)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Employee employee = await db.Employee.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(new EmployeeModel(employee));
        }

        // POST: Providers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            Employee employee = await db.Employee.FindAsync(id);
            db.Employee.Remove(employee);
            await db.SaveChangesAsync();

            if (employee.File != null)
            {
                if (employee.File.ID > ((int)FileType.Max - 1))
                {
                    await FilesController.DeleteAsync(employee.File.ID);
                }
            }
            return RedirectToAction("List", "Employees");
        }
    }
}
