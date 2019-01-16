using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace DBWT.Models
{
    public class Registrierung
    {
        public string Role { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Geburtsjahr { get; set; }
        public string Geburtsmonat { get; set; }
        public string Geburtstag { get; set; }
        public string Email { get; set; }
        public string Nutzername { get; set; }
        public string Password { get; set; }
        public string PasswordVerify { get; set; }

        public string Studiengang { get; set; }
        public string Matrikelnummer { get; set; }

        public string Buero { get; set; }
        public string Telefon { get; set; }

        public string Grund { get; set; }

        public string UserMessage = "";
        public string UserMessageStatus = "";

        public void NewUser()
        {
            string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(dbConStr);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction transaction = con.BeginTransaction();
            cmd.Connection = con;
            cmd.Transaction = transaction;
            if (!string.IsNullOrEmpty(Vorname))
            {
                int Nummer = 0;
                string hash = PasswordSecurity.PasswordStorage.CreateHash(Password);
                string[] split = hash.Split(':');
                try
                {
                    cmd.CommandText = "SELECT MAX(Nummer) FROM Benutzer";
                    MySqlDataReader r = cmd.ExecuteReader();
                    r.Read();
                    Nummer = (int)r[0] + 1;
                    r.Close();

                    DateTime t = DateTime.Now;
                    string Anlegedatum = @t.ToString("yyyy") + "-" + @t.ToString("MM") + "-" + @t.ToString("dd");

                    if(!string.IsNullOrEmpty(Geburtstag) && !string.IsNullOrEmpty(Geburtsmonat) && !string.IsNullOrEmpty(Geburtsjahr))
                    {
                        string Geburtsdatum = Geburtsjahr + "-" + Geburtsmonat + "-" + Geburtstag;
                        string GeburtsdatumC = Geburtsjahr + "/" + Geburtsmonat + "/" + Geburtstag + " 12:00:00 AM";
                        DateTime GeburtsdatumDT = DateTime.Parse(GeburtsdatumC);
                        TimeSpan diff = t.Subtract(GeburtsdatumDT);
                        int alter = diff.Days / 365;

                        cmd.CommandText = "INSERT INTO Benutzer (Nutzername, Vorname, Nachname, Geburtsdatum, `Alter`, Salt, Hash, EMail, Aktiv, Nummer, Anlegedatum) VALUES ('" +
                            Nutzername + "','" + Vorname + "','" + Nachname + "','" + Geburtsdatum + "','" + alter + "','" + split[3] + "','" + split[4] + "','" + Email + "', '0', '" + Nummer + "','" + Anlegedatum + "')";
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO Benutzer (Nutzername, Vorname, Nachname, Salt, Hash, EMail, Aktiv, Nummer, Anlegedatum) VALUES ('" +
                            Nutzername + "','" + Vorname + "','" + Nachname + "','"  + split[3] + "','" + split[4] + "','" + Email + "', '0', '" + Nummer + "','" + Anlegedatum + "')";
                    }

                    int rows = cmd.ExecuteNonQuery();
                    if (Role == "Gast")
                    {
                        DateTime AblaufdatumC = t.AddYears(1);
                        string Ablaufdatum = @AblaufdatumC.ToString("yyyy") + "-" + @AblaufdatumC.ToString("MM") + "-" + @AblaufdatumC.ToString("dd");
                        cmd.CommandText = "INSERT INTO Gaeste (ID, Grund, Ablaufdatum) VALUES ('" + Nummer + "', '" + Grund + "','" + Ablaufdatum + "')";
                        cmd.ExecuteNonQuery();
                    }
                    if (Role == "Mitarbeiter")
                    {
                        cmd.CommandText = "INSERT INTO FHAngehoerige (ID) VALUES ('" + Nummer + "')";
                        cmd.ExecuteNonQuery();

                        if (!string.IsNullOrEmpty(Telefon) && !string.IsNullOrEmpty(Buero))
                        {
                            cmd.CommandText = "INSERT INTO Mitarbeiter (ID, Buero, Telefon) VALUES ('" + Nummer + "', '" + Buero + "', '" + Telefon + "')";
                        }
                        else if(!string.IsNullOrEmpty(Telefon))
                        {
                            cmd.CommandText = "INSERT INTO Mitarbeiter (ID, Telefon) VALUES ('" + Nummer + "', '" + Telefon + "')";
                        }
                        else if (!string.IsNullOrEmpty(Buero))
                        {
                            cmd.CommandText = "INSERT INTO Mitarbeiter (ID, Buero) VALUES ('" + Nummer + "', '" + Buero + "')";
                        }
                        else
                        {
                            cmd.CommandText = "INSERT INTO Mitarbeiter (ID) VALUES ('" + Nummer + "')";
                        }
                        cmd.ExecuteNonQuery();

                    }
                    if (Role == "Student")
                    {
                        cmd.CommandText = "INSERT INTO FHAngehoerige (ID) VALUES ('" + Nummer + "')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Studenten (ID, Studiengang, Matrikelnummer) VALUES ('" + Nummer + "', '" + Studiengang + "', '" + Matrikelnummer + "')";
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    UserMessageStatus = "true";
                    UserMessage = "Die Registrierung wurde erfolgreich abgeschlossen.";
                }
                catch (Exception)
                {
                    UserMessageStatus = "false";
                    UserMessage = "Die Registrierung konnte nicht abgeschlossen werden.";
                    transaction.Rollback();
                }
            }
        }
    }
}