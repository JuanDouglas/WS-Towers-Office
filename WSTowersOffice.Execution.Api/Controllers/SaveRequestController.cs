using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WSTowersOffice.Execution.Api.Models;

namespace WSTowersOffice.Execution.Api.Controllers
{
    public class SaveRequestController : ApiController
    {
        WSTowersOfficeEntities db = new WSTowersOfficeEntities();
        [HttpPost]
        public async Task<IHttpActionResult> NewRequest(string email) {
            if (!email.Contains('@'))
            {
                return BadRequest();
            }

            if (email.Length<5)
            {
                return BadRequest();
            }

            if (db.RegistroSolicitacao.FirstOrDefault(fs=>fs.Email==email)!=null)
            {
                return StatusCode(HttpStatusCode.Ambiguous);
            }

            db.RegistroSolicitacao.Add(new RegistroSolicitacao() { 
                Email = email, 
                Data = DateTime.Now
            });
            await db.SaveChangesAsync();
            return Ok();
        } 
    }
}
