using ShowProducts.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WSTowersOffice.Api.Models.Attributes;

namespace WSTowersOffice.Api.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return RedirectToActionPermanent("Authentication","Logins");
        }
        public async Task<ActionResult> About()
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

}