using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DataModels;
using DBWT.Models;

namespace DBWT.Controllers
{
    public class DispatchController : Controller
    {
        public ActionResult Authorisierung()
        {
            Request.Headers.Add("X-Authorize", "Authorisierung");
            return Bestellungen();
        }

        [HttpGet]
        public ActionResult Bestellungen()
        {
            if (HttpContext.Request.Headers.Get("X-Authorize") != null && HttpContext.Request.Headers.Get("X-Authorize").ToString() == "Authorisierung")
            {
                using (var database = new EmensaDB())
                {
                    var query = from ben in database.Benutzer
                                join best in database.Bestellungen
                                on ben.Nummer equals best.BenutzerNummer
                                where best.Abholzeitpunkt <= DateTime.Now.AddHours(1)
                                select new
                                {
                                    User = new
                                    {
                                        ben.Vorname,
                                        ben.Nachname,
                                        ben.Nutzername,
                                        ben.EMail
                                    },
                                    Abholung = (best.Abholzeitpunkt).ToString(),
                                    Bestellnummer = best.Nummer,
                                    Bestellung = new List<DispatchListe>()
                                };

                    var query2 = from pro in database.Mahlzeiten
                                 join kategorie in database.Kategorien
                                 on pro.KategorienID equals kategorie.ID
                                 join bestelldetails in database.Mahlzeitenmbestellungenn
                                 on pro.ID equals bestelldetails.MahlzeitenID
                                 join bestell in database.Bestellungen
                                 on bestelldetails.BestellungsNummer equals bestell.Nummer
                                 select new
                                 {
                                    bestelldetails.Anzahl,
                                    Kategorie = kategorie.Bezeichnung,
                                    pro.Name,
                                    pro.Vorrat,
                                    Bestellung = bestell.Nummer
                                 };

                    var jsonBest = query.ToList();
                    var jsonPro = query2.ToList();

                    foreach (var order in jsonBest)
                    {
                        foreach (var pro in jsonPro)
                        {
                            if (order.Bestellnummer == pro.Bestellung)
                            {
                                order.Bestellung.Add(new DispatchListe
                                {
                                    Anzahl = (int) pro.Anzahl,
                                    Kategorie = pro.Kategorie,
                                    Name = pro.Name,
                                    Vorrat = pro.Vorrat
                                });
                            }
                        }
                    }
                    return Json(jsonBest, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Response.StatusCode = 401;
                Response.StatusDescription = "Zugriff verweigert.";
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}