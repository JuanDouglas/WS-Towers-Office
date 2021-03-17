using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WSTowersOffice.Execution.Api.Models;

namespace WSTowersOffice.Execution.Api.Controllers
{
    public class SolicitacoesController : Controller
    {
        private WSTowersOfficeEntities db = new WSTowersOfficeEntities();

        // GET: Solicitacoes
        public async Task<ActionResult> Index()
        {
            return View(await db.RegistroSolicitacao.ToListAsync());
        }

        // GET: Solicitacoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistroSolicitacao registroSolicitacao = await db.RegistroSolicitacao.FindAsync(id);
            if (registroSolicitacao == null)
            {
                return HttpNotFound();
            }
            return View(registroSolicitacao);
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
