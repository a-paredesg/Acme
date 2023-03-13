using Acme.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Acme.Controllers
{
    [Authorize(Roles = "1,2")]
    public class CamposController : Controller
    {
        private AcmeContext db = new AcmeContext();

     
        public async Task<ActionResult> Index()
        {
            var campos = db.Campos.Include(c => c.encuesta);
            return View(await campos.ToListAsync());
        }

       
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campo campo = await db.Campos.FindAsync(id);
            if (campo == null)
            {
                return HttpNotFound();
            }
            return View(campo);
        }

       
        public ActionResult Create()
        {
            ViewBag.EncuestaId = new SelectList(db.Encuestas, "Id", "Nombre");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nombre,Titulo,EsRequerido,TipoCampo,EncuestaId")] Campo campo)
        {
            if (ModelState.IsValid)
            {
                db.Campos.Add(campo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EncuestaId = new SelectList(db.Encuestas, "Id", "Nombre", campo.EncuestaId);
            return View(campo);
        }

      
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campo campo = await db.Campos.FindAsync(id);
            if (campo == null)
            {
                return HttpNotFound();
            }
            ViewBag.EncuestaId = new SelectList(db.Encuestas, "Id", "Nombre", campo.EncuestaId);
            return View(campo);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nombre,Titulo,EsRequerido,TipoCampo,EncuestaId")] Campo campo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EncuestaId = new SelectList(db.Encuestas, "Id", "Nombre", campo.EncuestaId);
            return View(campo);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campo campo = await db.Campos.FindAsync(id);
            if (campo == null)
            {
                return HttpNotFound();
            }
            return View(campo);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Campo campo = await db.Campos.FindAsync(id);
            db.Campos.Remove(campo);
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
