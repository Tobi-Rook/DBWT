using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DBWT.Models
{
   
    public struct Zutat
    {
        public string name;
        public bool vegan, vegetarisch, glutenfrei, bio;

        public Zutat(string name, bool vegan, bool vegetarisch, bool glutenfrei, bool bio)
        {
            this.name = name;
            this.vegan = vegan;
            this.vegetarisch = vegetarisch;
            this.glutenfrei = glutenfrei;
            this.bio = bio;
        }
    }

    public class Zutatenliste
    {
        public List<Zutat> zutatenliste = new List<Zutat>();

        public void Liste()
        {
            string dbConStr = ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(dbConStr);
            con.Open();
            MySqlCommand cmd;
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Name, Vegan, Vegetarisch, Glutenfrei, Bio FROM Zutaten ORDER BY Bio DESC, Name ASC";
            MySqlDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                zutatenliste.Add(new Zutat(r["name"].ToString(), (bool)r["vegan"], (bool)r["vegetarisch"], (bool)r["glutenfrei"], (bool)r["bio"]));
            }
        }
    }

}