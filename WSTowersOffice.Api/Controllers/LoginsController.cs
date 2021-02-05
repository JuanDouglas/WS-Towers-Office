using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WSTowersOffice.Api.Controllers
{
    public class LoginsController : Controller
    {
        // GET: Logins
        public async Task<ActionResult> Authentication(string post)
        {
            ViewBag.isError = false;
            ViewBag.Post = post;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password, bool viewpage, string post)
        {
            return RedirectToAction("Authetication", "api/Login", new { user = email, pas = password, viewpage = viewpage, post = post });

        }
        public async Task<ActionResult> AuthenticationMessage(bool isError, string message, string title, string post)
        {
            ViewBag.isError = isError;
            ViewBag.Post = post;
            return View("Authentication");
        }
    }
}