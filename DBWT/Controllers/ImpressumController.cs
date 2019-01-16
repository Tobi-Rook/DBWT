using System.Web.Mvc;

namespace DBWT.Controllers
{
    public class ImpressumController : Controller
    {
        // GET: Impressum
        public ActionResult Index()
        {
            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View();
        }
    }
}