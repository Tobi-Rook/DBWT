using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DBWT.Models
{
    public struct Produkt
    {
        public int id;
        public string name;
        public Bild bild;
        public bool verfuegbar;

        public Produkt(int id, string name, Bild bild, bool verfuegbar)
        {
            this.id = id;
            this.name = name;
            this.bild = bild;
            this.verfuegbar = verfuegbar;
        }
    }

    public struct Oberkategorie
    {
        public string name;
        public List<Unterkategorie> unterkategorienliste;

        public Oberkategorie(string kategorie, string name)
        {
            this.name = name;
            unterkategorienliste = new List<Unterkategorie>();
        }
    }

    public struct Unterkategorie
    {
        public int id;
        public string name;

        public Unterkategorie(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public struct Bild
    {
        public string bild, titel, alttext;
        public Bild(string bild, string titel, string alttext)
        {
            this.bild = bild;
            this.titel = titel;
            this.alttext = alttext;
        }
    }

    public class Produkte
    {
        public string filter = "Alle anzeigen";
        public int KategorieG { get; set; }
        public string VeganG { get; set; }
        public string VegetarischG { get; set; }
        public string VerfuegbarG { get; set; }

        public List<Produkt> produkte = new List<Produkt>();
        public List<Oberkategorie> oberkategorienliste = new List<Oberkategorie>();

        public void KategorienListe()
        {
            string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(dbConStr);
            con.Open();
            MySqlCommand cmd;
            cmd = con.CreateCommand();
            cmd.CommandText =   "SELECT Unterkategorien.ID, Unterkategorien.UnterkategorieID, t1.Bezeichnung, t2.Bezeichnung FROM Unterkategorien " +
                                "JOIN Kategorien AS t1 ON Unterkategorien.ID = t1.ID " +
                                "JOIN Kategorien AS t2 ON Unterkategorien.UnterkategorieID = t2.ID";
            MySqlDataReader r = cmd.ExecuteReader();
            Oberkategorie oberkategorie = new Oberkategorie("", "");

            int last = 0;

            while(r.Read())
            {
                int oberkategorieID = (int)r["ID"];
                int unterkategorieID = (int)r["UnterkategorieID"];

                if (last != oberkategorieID) // Falls Oberkategorie neu in Liste
                {
                    oberkategorie = new Oberkategorie("", "")
                    {
                        name = r[2] as string
                    };
                    oberkategorienliste.Add(oberkategorie);
                }

                if (KategorieG == unterkategorieID)
                {
                    filter = r[3] as string;
                }

                last = (int)r["ID"];
                oberkategorie.unterkategorienliste.Add(new Unterkategorie((int)r[1], r[3].ToString()));
            }
            r.Close();
            con.Close();
        }

        public void ProdukteListe()
        {
            string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(dbConStr);
            con.Open();
            MySqlCommand cmd;
            cmd = con.CreateCommand();
            cmd.CommandText =   "SELECT Mahlzeiten.ID, Mahlzeiten.Name, Mahlzeiten.Verfuegbar, Mahlzeiten.KategorienID, " +
                                "Bilder.Titel, Bilder.Binaerdaten, Bilder.AltText, " +
                                "AVG(Zutaten.Vegetarisch), AVG(Zutaten.Vegan) FROM Mahlzeiten " +
                                "JOIN MahlzeitenMBilderN ON MahlzeitenMBilderN.MahlzeitenID = Mahlzeiten.ID " +
                                "JOIN Bilder ON MahlzeitenMBilderN.BildID = Bilder.ID " +
                                "JOIN MahlzeitenMZutatenN ON MahlzeitenMZutatenN.MahlzeitenID = Mahlzeiten.ID " +
                                "JOIN Zutaten ON MahlzeitenMZutatenN.ZutatenID = Zutaten.ID GROUP BY Mahlzeiten.ID";
            bool havingIncl = false;

            if (KategorieG != 0)
            {
                cmd.CommandText += " HAVING KategorienID = " + KategorieG;
                havingIncl = true;
            }

           if (VeganG == "on")
            {
                if (!havingIncl)
                {
                    cmd.CommandText += " HAVING AVG(Zutaten.Vegan) = 1";
                    havingIncl = true;
                }
                else
                {
                    cmd.CommandText += " AND AVG(Zutaten.Vegan) = 1";
                }
            }

            if (VegetarischG == "on")
            {
                if (!havingIncl)
                {
                    cmd.CommandText += " HAVING AVG(Zutaten.Vegetarisch) = 1";
                    havingIncl = true;
                }
                else
                {
                    cmd.CommandText += " AND AVG(Zutaten.Vegetarisch) = 1";
                }
            }

            if (VerfuegbarG == "on")
            {
                if (!havingIncl)
                {
                    cmd.CommandText += " HAVING Mahlzeiten.Verfuegbar = 1";
                    havingIncl = true;
                }
                else
                {
                    cmd.CommandText += " AND Mahlzeiten.Verfuegbar = 1";
                }
            }
            
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                byte[] bild = (byte[])r["Binaerdaten"];
                string base64 = "data:image/jpeg;base64," + Convert.ToBase64String(bild);
                produkte.Add(new Produkt((int)r["ID"], r["Name"] as string, new Bild(base64, r["Titel"] as String, r["AltText"] as String), (bool)r["Verfuegbar"]));
            }
            r.Close();
            con.Close();
        }
    }
}