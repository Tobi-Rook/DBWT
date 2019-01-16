using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace DBWT.Models
{
    public class Login
    {
        [Required]
        public string user { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public bool signedIn { get; set; }
        public bool signout { get; set; }
        public bool verified { get; set; }

        public void Password()
        {
            if (!signedIn && !signout)
            {
                string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
                MySqlConnection con = new MySqlConnection(dbConStr);
                con.Open();
                MySqlCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Benutzer.Nummer, Benutzer.Nutzername,'Student' AS Rolle, Benutzer.Hash, Benutzer.Salt, Benutzer.Aktiv FROM Benutzer JOIN Studenten ON Benutzer.Nummer = Studenten.ID WHERE Benutzer.Nutzername = '" + user + "' UNION ";
                cmd.CommandText += "SELECT Benutzer.Nummer, Benutzer.Nutzername,'Mitarbeiter' AS Rolle, Benutzer.Hash, Benutzer.Salt, Benutzer.Aktiv FROM Benutzer JOIN Mitarbeiter ON Benutzer.Nummer = Mitarbeiter.ID WHERE Benutzer.Nutzername = '" + user + "' UNION ";
                cmd.CommandText += "SELECT Benutzer.Nummer, Benutzer.Nutzername,'Gast' AS Rolle, Benutzer.Hash, Benutzer.Salt, Benutzer.Aktiv FROM Benutzer JOIN Gaeste ON Benutzer.Nummer = Gaeste.ID WHERE Benutzer.Nutzername = '" + user + "'";
                MySqlDataReader r = cmd.ExecuteReader();
                if (!string.IsNullOrEmpty(password) && r.Read())
                {
                    string hash = "sha1:64000:18:" + r["Salt"] + ":" + r["Hash"];
                    if (PasswordSecurity.PasswordStorage.VerifyPassword(password, hash) && r["Aktiv"].ToString() == "1")
                    {
                        verified = true;
                        user = r["Nutzername"] as string;
                        role = r["Rolle"] as string;
                        signedIn = true;
                    }
                }
                r.Close();
                con.Close();
            }
            else if (signout)
            {
                signedIn = false;
                user = "";
                role = "Gast";
            }
            else
            {
                signedIn = false;
                user = "";
                role = "Gast";
            }
        }
    }
}