using System.Web.Mvc;
using DBWT.Models;

namespace DBWT.Controllers
{
    public class ProdukteController : Controller
    {
        // GET: Produkte
        public ActionResult Index()
        {
            Produkte pro = new Produkte();

            if(Request.QueryString["kategorie"] != null)
            {
                pro.KategorieG = short.Parse(Request.QueryString["kategorie"]);
            }
            else
            {
                pro.KategorieG = 0;
            }

            pro.VeganG = Request.QueryString["vegan"];
            pro.VegetarischG = Request.QueryString["vegetarisch"];
            pro.VerfuegbarG = Request.QueryString["verfuegbar"];

            pro.ProdukteListe();
            pro.KategorienListe();

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View(pro);
        }
    }
}