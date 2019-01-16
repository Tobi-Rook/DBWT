using System.Collections.Specialized;
using System.Web.Mvc;

using DBWT.Models;
using DataModels;

namespace DBWT.Controllers
{
    public class BestellungenController : Controller
    {
        // GET: Bestellung

        public ActionResult Index()
        {
            NameValueCollection nvc = Request.Form;

            Models.Bestellungen best = new Models.Bestellungen();
            best.changed = false;
            DataModels.Bestellungen bestData = new DataModels.Bestellungen();
            Mahlzeitenmbestellungenn mbestData = new Mahlzeitenmbestellungenn();

            string user = Session["user"] as string;

            best.logRole = "Gast";
            if (!string.IsNullOrEmpty(Session["role"] as string))
            {
                best.logRole = Session["role"] as string;
            }

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session, best);

            foreach (Artikel artikel in best.artikelListe)
            {
                best.gesamtpreis += artikel.anzahl * artikel.preis;
            }

            if (nvc["delete"] == "Alle löschen")
            {
                CookieManagement.ResetCookie(HttpContext, Session, nvc);
                ViewBag.Anzahl = 0;
                best.changed = true;
                best.UserMessage = "Bestellung wird gelöscht...";
                best.UserMessageStatus = "wait";
            }

            if(nvc["update"] == "Änderungen übernehmen")
            {
                if(CookieManagement.ChangeCookie(HttpContext, Session, nvc))
                {
                    best.changed = true;
                    best.UserMessage = "Bestellung wird geändert...";
                    best.UserMessageStatus = "wait";
                }
                else
                {
                    best.UserMessage = "Ihre Änderungsanfrage enthielt Bestellanzahlen, die die maximale Bestellmenge übersteigen würden.";
                    best.UserMessageStatus = "false";
                }
            }

            if (nvc["order"] == "order")
            {
                string[] zeit = (nvc["zeit"] as string).Split(',');
                best.Order(bestData, mbestData, user, zeit);
            }

            return View(best);
        }
    }
}