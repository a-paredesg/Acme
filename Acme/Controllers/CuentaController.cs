using Acme.Models;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Acme.Controllers
{
    public class CuentaController : Controller
    {
        private AcmeContext db = new AcmeContext();
        private const string AuthenticationType = "ApplicationCookie";

        public ActionResult Iniciar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iniciar(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var usuario = (from u in db.Usuarios
                               where u.Email.Equals(email)
                               select u).FirstOrDefault();

                if (usuario != null)
                {
                    if (!SecurityPassword.PasswordStorage.VerifyPassword(password, usuario.Password))
                    {
                        ViewBag.Error = "Usuario o contraseña incorrecto";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrecto";
                    return View();
                }
                Session["User"] = usuario;

                IList<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.RolId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Email )
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationType);

                IAuthenticationManager authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;

                authenticationManager.SignIn(identity);



                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult Salir()
        {
            Session["User"] = null;
            IAuthenticationManager authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;

            authenticationManager.SignOut(AuthenticationType);
            return View("../Cuenta/Iniciar");
        }

        /
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,ConfirmPassword")] Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string pw = SecurityPassword.PasswordStorage.CreateHash(usuario.Password);
                    usuario.Password = pw;
                    usuario.ConfirmPassword = pw;
                    usuario.RolId = db.Roles.Where(x => x.Nombre.Equals("Usuario")).First().Id;
                    db.Usuarios.Add(usuario);
                    db.SaveChanges();
                    ViewBag.Success = "Usuario creado exitosamente";
                }
                else
                {
                    ViewBag.Error = "El usuario no se ha creado.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(usuario);
        }

      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,RolId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

    
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
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
