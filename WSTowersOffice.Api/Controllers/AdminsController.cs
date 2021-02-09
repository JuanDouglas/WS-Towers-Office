using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WSTowersOffice.Api.Models;
using ShowProducts.API.Controllers;

namespace WSTowersOffice.Api.Controllers
{
    public class AdminsController : Controller
    {
        private WSTowersOfficeEntities db = new WSTowersOfficeEntities();

        // GET: Admins
        public async Task<ActionResult> Manager()
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            return View(await db.Login.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<ActionResult> Details(int? id)
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
            Login login = await db.Login.FindAsync(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // GET: Admins/Create
        public async Task<ActionResult> Create()
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            return View();
        }

        // POST: Admins/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,UserName,Password,Email,isValidEmail,CreateDate,ValidKey")] LoginModel login)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }

            if ((await db.Login.FirstOrDefaultAsync(fs => fs.Email == login.Email)) != null)
            {
                ModelState.AddModelError("Email", "This 'e-mail' already ben using.");
            }

            if ((await db.Login.FirstOrDefaultAsync(fs => fs.UserName == login.UserName)) != null)
            {
                ModelState.AddModelError("Username", "This 'Username' already ben using.");
            }

            Login loginModel = login.GetLogin();

            if (login.Password != login.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password confirmation of being equal to password");
            }
            loginModel.Password = LoginController.CryptographyString(login.Password);
            loginModel.CreateDate = DateTime.UtcNow;

            Guid userkey = Guid.NewGuid();
            bool exist = false;
            do
            {
                if ((await db.Login.FirstOrDefaultAsync(fs => fs.ValidKey == userkey.ToString())) == null)
                {
                    exist = true;
                    userkey = Guid.NewGuid();
                }
            } while (exist);

            loginModel.ValidKey = userkey.ToString();
            if (ModelState.IsValid)
            {
                db.Login.Add(loginModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(login);
        }

        // GET: Admins/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            Login login = await db.Login.FindAsync(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // POST: Admins/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UserName,Password,Email,isValidEmail,CreateDate,ValidKey")] Login login)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            if (ModelState.IsValid)
            {
                db.Entry(login).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(login);
        }

        // GET: Admins/Delete/5
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
            Login login = await db.Login.FindAsync(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var loginResult = await LoginController.ValidLoginAsync();
            if (!loginResult.IsValid)
            {
                return RedirectToActionPermanent("Authentication", "Logins");
            }
            Login login = await db.Login.FindAsync(id);
            db.Login.Remove(login);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
