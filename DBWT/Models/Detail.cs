using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;
using LinqToDB;
using DataModels;

namespace DBWT.Models
{
    public class Detail
    {
        public Produkt pro;
        public string beschreibung;
        public List<string> zutaten = new List<string>();
        public int ID { get; set; }
        public Login log;
        public double preis;
        public bool gueltig;
        public bool ordered;

        public string UserMessage = "";
        public string UserMessageStatus = "";

        public void Details()
        {
            string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(dbConStr);
            con.Open();
            MySqlCommand cmd;
            cmd = con.CreateCommand();
            cmd.CommandText =   "SELECT Mahlzeiten.ID, Mahlzeiten.Name, Mahlzeiten.Beschreibung, Mahlzeiten.Verfuegbar, " +
                                "Bilder.Titel, Bilder.Binaerdaten, Bilder.AltText, " +
                                "Preise.Studentpreis, Preise.MAPreis, Preise.Gastpreis FROM Mahlzeiten " +
                                "JOIN MahlzeitenMBilderN ON Mahlzeiten.ID = MahlzeitenMBilderN.MahlzeitenID " +
                                "JOIN Bilder ON Bilder.ID = MahlzeitenMBilderN.BildID " +
                                "JOIN Preise ON Preise.MahlzeitenID = Mahlzeiten.ID " +
                                "WHERE Mahlzeiten.ID = '" + ID + "'";
            MySqlDataReader r = cmd.ExecuteReader();
            r.Read();
            try
            {
                gueltig = true;
                byte[] bild = (byte[])r["Binaerdaten"];
                string base64 = "data:image/jpeg;base64," + Convert.ToBase64String(bild);
                pro = new Produkt((int)r["ID"], r["Name"] as string, new Bild(base64, r["Titel"] as string, r["AltText"] as string), (bool)r["Verfuegbar"]);
                beschreibung = r["Beschreibung"] as string;
                if (log.role == "Student")
                {
                    preis = Convert.ToDouble(r["Studentpreis"]);
                }
                else if (log.role == "Mitarbeiter")
                {
                    preis = Convert.ToDouble(r["MAPreis"]);
                }
                else
                {
                    preis = Convert.ToDouble(r["Gastpreis"]);
                }
            }
            catch
            {
                gueltig = false;
            }
            r.Close();
            con.Close();
        }

        public void Zutaten()
        {
            string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(dbConStr);
            con.Open();
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Name FROM Zutaten " +
                "JOIN MahlzeitenMZutatenN on Zutaten.ID = MahlzeitenMZutatenN.ZutatenID " +
                "WHERE MahlzeitenMZutatenN.MahlzeitenID = '" + ID + "'";
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                zutaten.Add(r["name"] as string);
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