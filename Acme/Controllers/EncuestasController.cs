using Acme.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Acme.Controllers
{
    [Authorize(Roles = "1,2")]
    public class EncuestasController : Controller
    {
        private AcmeContext db = new AcmeContext();

        // GET: Encuestas
        public ActionResult Index()
        {
            var campos = db.Campos.ToList();
            var encuestas = db.Encuestas.ToList();
            return View(encuestas);
        }

        // GET: Encuestas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Encuesta encuesta = await db.Encuestas.FindAsync(id);
            var campos = db.Campos.Where(x => x.EncuestaId == id);

            foreach (Campo item in campos)
            {
                encuesta.Campos.Add(item);
            }

            if (encuesta == null)
            {
                return HttpNotFound();
            }
            return View(encuesta);
        }

        // GET: Encuestas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Encuestas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nombre,Descripcion")] Encuesta encuesta)
        {
            if (ModelState.IsValid)
            {
                db.Encuestas.Add(encuesta);
                await db.SaveChangesAsync();
                return RedirectToAction($"../Encuestas/Index");
            }

            return View(encuesta);
        }

        // GET: Encuestas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Encuesta encuesta = await db.Encuestas.FindAsync(id);
            if (encuesta == null)
            {
                return HttpNotFound();
            }
            return View(encuesta);
        }

        // POST: Encuestas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nombre,Descripcion")] Encuesta encuesta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(encuesta).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(encuesta);
        }

        // GET: Encuestas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Encuesta encuesta = await db.Encuestas.FindAsync(id);
            if (encuesta == null)
            {
                return HttpNotFound();
            }
            return View(encuesta);
        }

        // POST: Encuestas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Encuesta encuesta = await db.Encuestas.FindAsync(id);
            db.Encuestas.Remove(encuesta);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> CreateUrl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var campos = db.Campos.Where(x => x.EncuestaId == id);
            Encuesta encuesta = await db.Encuestas.FindAsync(id);

            foreach (var item in campos)
            {
                encuesta.Campos.Add(item);
            }

            if (String.IsNullOrEmpty(encuesta.Url))
            {
                encuesta.Url = $"/Encuestas/Completar/{id}";
                db.Entry(encuesta).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            string tabla = $"create table {encuesta.Nombre.Trim().ToLower().Replace(" ", "")}" +
                $"(id int primary key IDENTITY(1, 1),";
            string aux = "";
            foreach (var item in campos)
            {
                switch (item.TipoCampo)
                {
                    case TipoCampo.Texto:
                        if (item.EsRequerido)
                        {
                            aux += aux.Equals(string.Empty)
                                ? $"{item.Nombre.Trim().ToLower().Replace(" ", "_")} varchar(200) NOT NULL"
                                : $",{item.Nombre.Trim().ToLower().Replace(" ", "_")} varchar(200) NOT NULL";
                        }
                        else
                        {
                            aux += aux.Equals(string.Empty)
                                ? $"{item.Nombre.Trim().ToLower().Replace(" ", "_")} varchar(200)"
                                : $",{item.Nombre.Trim().ToLower().Replace(" ", "_")} varchar(200)";
                        }
                        break;
                    case TipoCampo.Numero:
                        if (item.EsRequerido)
                        {
                            aux += aux.Equals(string.Empty)
                                ? $"{item.Nombre.Trim().ToLower().Replace(" ", "_")} int NOT NULL"
                                : $",{item.Nombre.Trim().ToLower().Replace(" ", "_")} int NOT NULL";
                        }
                        else
                        {
                            aux += aux.Equals(string.Empty)
                                ? $"{item.Nombre.Trim().ToLower().Replace(" ", "_")} int"
                                : $",{item.Nombre.Trim().ToLower().Replace(" ", "_")} int";
                        }
                        break;
                    case TipoCampo.Fecha:
                        if (item.EsRequerido)
                        {
                            aux += aux.Equals(string.Empty)
                                ? $"{item.Nombre.Trim().ToLower().Replace(" ", "_")} date NOT NULL"
                                : $",{item.Nombre.Trim().ToLower().Replace(" ", "_")} date NOT NULL";
                        }
                        else
                        {
                            aux += aux.Equals(string.Empty)
                                ? $"{item.Nombre.Trim().ToLower().Replace(" ", "_")} date"
                                : $",{item.Nombre.Trim().ToLower().Replace(" ", "_")} date";
                        }
                        break;
                }

            }
            tabla += aux + $")";

            var sqlcommand = String.Format(tabla);
            db.Database.ExecuteSqlCommand(sqlcommand);
            return RedirectToAction("Index");
        }

        public ActionResult AgregarCampo(int id)
        {
            Campo campo = new Campo();
            campo.EncuestaId = id;
            ViewBag.EncuestaId = new SelectList(db.Encuestas, "Id", "Nombre", campo.EncuestaId);
            return View(campo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarCampo([Bind(Include = "Id,Nombre,Titulo,EsRequerido,TipoCampo,EncuestaId")] Campo campo)
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Completar(int id)
        {
            var encuesta = db.Encuestas.FirstOrDefault(x => x.Id == id);
            var campos = db.Campos.Where(x => x.EncuestaId == id).ToList();
            if (encuesta != null && encuesta.Url != null)
            {
                return View(encuesta);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Completar()
        {
            int id = int.Parse(Request.Form["id"]);
            var encuesta = db.Encuestas.FirstOrDefault(x => x.Id == id);
            var campos = db.Campos.Where(x => x.EncuestaId == id);

            try
            {
                string sqlcommand = string.Empty;
                string fileds = string.Empty;
                string values = string.Empty;

                sqlcommand += $"insert into [Encuestas].dbo.{encuesta.Nombre.ToLower().Replace(" ", "").Trim()} ";

                foreach (var item in campos)
                {
                    string tipo = item.TipoCampo.ToString();
                    bool esRequerido = item.EsRequerido;

                    string campo = item.Nombre.ToLower().Replace(" ", "_");
                    string valor = Request.Form[item.Nombre];

                    fileds += fileds == string.Empty
                        ? campo
                        : $",{campo}";

                    values += values == string.Empty
                        ? valor.Equals("") ? "NULL" : tipo.Equals("Numero") ? valor : $"'{valor}'"
                        : valor.Equals("") ? ",NULL" : tipo.Equals("Numero") ? $",{valor}" : $",'{valor}'";

                }

                sqlcommand += $"({fileds}) ";
                sqlcommand += "values ";
                sqlcommand += $"({values}) ";

                var command = String.Format(sqlcommand);
                db.Database.ExecuteSqlCommand(command);
                ViewBag.Message = "Se ha enviado satisfactoriamente la encuesta.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(encuesta);
        }

        [HttpGet]
        public ActionResult ObtenerResultadosEncuesta(int id)
        {
            var bd = db.Encuestas.FirstOrDefault(x => x.Id == id).Nombre.ToLower().Replace(" ", "");
            var campos = db.Campos.Where(x => x.EncuestaId == id);
            string[] fields = new string[campos.Count()];

            int i = 0;
            foreach (var item in campos)
            {
                fields[i] = item.Nombre.ToLower().Replace(" ", "_");
                i++;
            }
            ViewBag.Collection = fields;

            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["AcmeContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = $"SELECT * FROM [Acme].dbo.{bd}";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                }
            }
            return View(ds);
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
