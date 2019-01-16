using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LinqToDB;
using MySql.Data.MySqlClient;
using DataModels;

namespace DBWT.Models
{
    public struct Artikel
    {
        public string name;
        public int ID;
        public decimal preis;
        public int anzahl;

        public Artikel(string name, int ID, decimal preis, int anzahl)
        {
            this.name = name;
            this.ID = ID;
            this.preis = preis;
            this.anzahl = anzahl;
        }
    }
    public class Bestellungen
    {
        public string logRole;

        public decimal gesamtpreis;

        public List<Artikel> artikelListe = new List<Artikel>();

        public string UserMessage = "";
        public string UserMessageStatus = "";

        public int anzahl;

        public bool changed;

        public void Artikelliste(Dictionary<int, int> tupel)
        {
            List<KeyValuePair<int, int>> artikelliste = tupel.ToList();
            anzahl = 0;
            foreach (KeyValuePair<int, int> artikel in artikelliste)
            {
                anzahl++;
                string name = "";
                decimal preis = 0;
                using (var MensaContext = new EmensaDB())
                {
                    var query = from Mahlzeiten in MensaContext.Mahlzeiten
                                where artikel.Key == Mahlzeiten.ID
                                select Mahlzeiten.Name;
                    name = query.First().ToString();

                    if (logRole == "Student")
                    {
                        var query2 = from Preise in MensaContext.Preise
                                     where artikel.Key == Preise.MahlzeitenID
                                     select Preise.Studentpreis;
                        preis = (decimal) query2.First();
                    }
                    else if (logRole == "Mitarbeiter")
                    {
                        var query2 = from Preise in MensaContext.Preise
                                     where artikel.Key == Preise.MahlzeitenID
                                     select Preise.MAPreis;
                        preis = (decimal) query2.First();
                    }
                    else
                    {
                        var query2 = from Preise in MensaContext.Preise
                                     where artikel.Key == Preise.MahlzeitenID
                                     select Preise.Gastpreis;
                        preis = query2.First();
                    }
                }
                Artikel a = new Artikel(name, artikel.Key, preis, artikel.Value);
                artikelListe.Add(a);
            }
        }

        public void Order(DataModels.Bestellungen bestData, Mahlzeitenmbestellungenn mbestData, string user, string[] zeit)
        {
            using (var database = new EmensaDB())
            {
                try
                {
                    database.BeginTransaction();

                    // Atrribute des Datenmodels Bestellungen

                    var query = from Benutzer in database.Benutzer
                                where Benutzer.Nutzername == user
                                select Benutzer.Nummer;
                    bestData.BenutzerNummer = query.First();

                    bestData.Bestellzeitpunkt = DateTime.Now;

                    DateTime abholzeitpunkt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(zeit[0]), int.Parse(zeit[1]), 0);
                    bestData.Abholzeitpunkt = abholzeitpunkt;

                    bestData.Endpreis = gesamtpreis;

                    var newID = database.InsertWithInt32Identity(bestData);

                    // Attribute des Datenmodels MahlzeitenMBestellungenN

                    List<int> list = new List<int>();
                    var query2 = from Bestellungen in database.Bestellungen
                                 select Bestellungen.Nummer;
                    list = query2.ToList();
                    mbestData.BestellungsNummer = list.Last();

                    foreach (Artikel artikel in artikelListe)
                    {
                        mbestData.MahlzeitenID = artikel.ID;
                        mbestData.Anzahl = artikel.anzahl;
                        var newID2 = database.InsertWithInt32Identity(mbestData);
                    }

                    database.CommitTransaction();

                    UserMessageStatus = "true";
                    UserMessage = "Die Bestellung wurde erfolgreich aufgegeben.";
                }
                catch
                {
                    UserMessageStatus = "false";
                    UserMessage = "Der Bestellvorgang ist fehlgeschlagen.";
                }
            }
        }

        public int DBAnzahl(int artikelID)
        {
            using (var MensaContext = new EmensaDB())
            {
                var query = from Mahlzeiten in MensaContext.Mahlzeiten
                            where artikelID == Mahlzeiten.ID
                            select Mahlzeiten.Vorrat;

                return query.First();
            }
        }
    }
}