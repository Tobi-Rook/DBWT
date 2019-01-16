using System.Web.Mvc;

using DBWT.Models;

namespace DBWT.Controllers
{
    public class ZutatenlisteController : Controller
    {
        // GET: Zutatenliste
        public ActionResult Index()
        {
            Zutatenliste zlist = new Zutatenliste();

            zlist.Liste();

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View(zlist);
        }
    }
}