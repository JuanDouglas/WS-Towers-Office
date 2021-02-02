using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WSTowersOffice.Api.Models;

namespace WSTowersOffice.Api.Controllers
{
    public class EmployeesController : Controller
    {
        private WSTowersOfficeEntities db = new WSTowersOfficeEntities();
        public async Task<ActionResult> Index() 
        {
            List<Employee> employees = await db.Employee.Where(wh=>true).ToListAsync();
            List<EmployeeModel> employeeModels = new List<EmployeeModel>();
            foreach (Employee employee in employees)
            {
                employeeModels.Add(new EmployeeModel(employee));
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "")]EmployeeModel employeeModel) {
          
            return View();
        }

        public async Task<ActionResult> AddProfileImage(int employee_id)
        {
            return RedirectToAction("","");
        }
    }
}
