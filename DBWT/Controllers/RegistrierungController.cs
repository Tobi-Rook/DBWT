using System.Web.Mvc;
using System.Collections.Specialized;

using DBWT.Models;

namespace DBWT.Controllers
{
    public class RegistrierungController : Controller
    {
        // GET: Registrierung
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Student()
        {
            NameValueCollection nvc = Request.Form;

            Registrierung reg = new Registrierung
            {
                Role = "Student",
                Vorname = nvc["vorname"],
                Nachname = nvc["nachname"],
                Geburtsjahr = nvc["geburtsjahr"],
                Geburtsmonat = nvc["geburtsmonat"],
                Geburtstag = nvc["geburtstag"],

                Email = nvc["email"],
                Nutzername = nvc["nutzername"],
                Password = nvc["password"],
                PasswordVerify = nvc["passwordVerify"],

                Studiengang = nvc["studiengang"],
                Matrikelnummer = nvc["matrikelnummer"]
            };

            reg.NewUser();

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View(reg);
        }

        public ActionResult Mitarbeiter()
        {
            NameValueCollection nvc = Request.Form;

            Registrierung reg = new Registrierung
            {
                Role = "Mitarbeiter",
                Vorname = nvc["vorname"],
                Nachname = nvc["nachname"],
                Geburtsjahr = nvc["geburtsjahr"],
                Geburtsmonat = nvc["geburtsmonat"],
                Geburtstag = nvc["geburtstag"],

                Email = nvc["email"],
                Nutzername = nvc["nutzername"],
                Password = nvc["password"],
                PasswordVerify = nvc["passwordVerify"],

                Buero = nvc["buero"],
                Telefon = nvc["telefon"]
            };

            reg.NewUser();

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View(reg);
        }

        public ActionResult Gast()
        {
            NameValueCollection nvc = Request.Form;

            Registrierung reg = new Registrierung
            {
                Role = "Gast",
                Vorname = nvc["vorname"],
                Nachname = nvc["nachname"],
                Geburtsjahr = nvc["geburtsjahr"],
                Geburtsmonat = nvc["geburtsmonat"],
                Geburtstag = nvc["geburtstag"],

                Email = nvc["email"],
                Nutzername = nvc["nutzername"],
                Password = nvc["password"],
                PasswordVerify = nvc["passwordVerify"],

                Grund = nvc["grund"]
            };

            reg.NewUser();

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            return View(reg);
        }
    }
}