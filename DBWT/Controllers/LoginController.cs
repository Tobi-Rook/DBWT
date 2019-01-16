using System.Web.Mvc;
using System.Collections.Specialized;
using DBWT.Models;

namespace DBWT.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            NameValueCollection nvc = Request.Form;

            Login log = new Login();
            
            LoginManagement.Login(Session, nvc, log);
            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View(log);
        }
    }
}