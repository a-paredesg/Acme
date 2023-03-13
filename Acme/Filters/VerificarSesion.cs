using Acme.Controllers;
using Acme.Models;
using System.Web;
using System.Web.Mvc;

namespace Acme.Filters
{
    public class VerificarSesion : ActionFilterAttribute
    {
        private Usuario usuario;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                usuario = (Usuario)HttpContext.Current.Session["User"];
                if (usuario == null)
                {
                    if (filterContext.Controller is CuentaController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("/Cuenta/Iniciar");
                    }

                }
            }
            catch (System.Exception)
            {
                filterContext.Result = new RedirectResult("/Index");
            }
        }
    }
}