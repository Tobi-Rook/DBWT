using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

using DBWT.Models;

using Newtonsoft.Json;

namespace DBWT
{
    public class CookieManagement
    {
        public class CookieStrucutre
        {
            public string user;
            public int gesamtanzahl;
            public Dictionary<int, int> artikel;
            public CookieStrucutre(string user)
            {
                this.user = user;
                gesamtanzahl = 0;
                artikel = new Dictionary<int, int>();
            }
        }

        public static int GetAnzahl(HttpContextBase context, HttpSessionStateBase session)
        {
            if (!string.IsNullOrEmpty(session["user"] as string))
            {
                if (context.Request.Cookies.Get(session["user"] as string) != null && context.Request.Cookies.Get(session["user"] as string).Value != null
                    && context.Request.Cookies.Get(session["user"] as string).Value != "")
                {
                    HttpCookie warenkorb = context.Request.Cookies.Get(session["user"] as string);
                    CookieStrucutre cookie = JsonConvert.DeserializeObject<CookieStrucutre>(warenkorb.Value);
                    if(cookie.artikel != null)
                    {
                        cookie.gesamtanzahl = 0;
                        foreach (KeyValuePair<int, int> pair in cookie.artikel)
                        {
                            cookie.gesamtanzahl += pair.Value;
                        }
                        warenkorb.Value = JsonConvert.SerializeObject(cookie);
                        warenkorb.Expires = DateTime.Now.AddDays(1);
                        context.Response.Cookies.Set(warenkorb);
                        return cookie.gesamtanzahl;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        public static int GetAnzahl(HttpContextBase context, HttpSessionStateBase session, Bestellungen best)
        {
            if (!string.IsNullOrEmpty(session["user"] as string))
            {
                string benutzername = session["user"] as string;
                if (context.Request.Cookies.Get(session["user"] as string) != null && context.Request.Cookies.Get(session["user"] as string).Value != null
                    && context.Request.Cookies.Get(session["user"] as string).Value != "")
                {
                    HttpCookie warenkorb = context.Request.Cookies.Get(benutzername);
                    CookieStrucutre cookie = JsonConvert.DeserializeObject<CookieStrucutre>(warenkorb.Value);
                    if (cookie.artikel != null)
                    {
                        cookie.gesamtanzahl = 0;
                        foreach (KeyValuePair<int, int> pair in cookie.artikel)
                        {
                            cookie.gesamtanzahl += pair.Value;
                        }
                        best.Artikelliste(cookie.artikel);
                        warenkorb.Value = JsonConvert.SerializeObject(cookie);
                        warenkorb.Expires = DateTime.Now.AddDays(1);
                        context.Response.Cookies.Set(warenkorb);
                        return cookie.gesamtanzahl;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        public static bool AddToCookie(HttpContextBase context, HttpSessionStateBase session, NameValueCollection nvc)
        {
            if (!string.IsNullOrEmpty(session["user"] as string) && !string.IsNullOrEmpty(nvc["proID"]))
            {
                int nummer = int.Parse(nvc["proID"]);
                HttpCookie warenkorb;
                CookieStrucutre cookie;
                if (context.Request.Cookies.Get(session["user"] as string) != null && context.Request.Cookies.Get(session["user"] as string).Value != null
                    && context.Request.Cookies.Get(session["user"] as string).Value != "")
                {
                    warenkorb = context.Request.Cookies.Get(session["user"] as string);
                    cookie = JsonConvert.DeserializeObject<CookieStrucutre>(warenkorb.Value);
                    if (!cookie.artikel.ContainsKey(nummer))
                    {
                        cookie.artikel.Add(nummer, 1);
                    }
                    else
                    {
                        Detail d = new Detail();
                        int amount = cookie.artikel[nummer];
                        if ((d.DBAnzahl(nummer) - amount) > 0)
                        {
                            cookie.artikel.Remove(nummer);
                            cookie.artikel.Add(nummer, amount + 1);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    warenkorb = new HttpCookie(session["user"] as string);
                    cookie = new CookieStrucutre(session["user"] as string);
                    cookie.artikel.Add(nummer, 1);
                }
                cookie.gesamtanzahl++;
                warenkorb.Value = JsonConvert.SerializeObject(cookie);
                warenkorb.Expires = DateTime.Now.AddDays(1);
                context.Response.Cookies.Set(warenkorb);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ChangeCookie(HttpContextBase context, HttpSessionStateBase session, NameValueCollection nvc)
        {
            Dictionary<int, int> currentDict = new Dictionary<int, int>();
            Dictionary<int, int> newDict = new Dictionary<int, int>();
            Bestellungen best = new Bestellungen();

            if (context.Request.Cookies.Get(session["user"] as string) == null)
            {
                return false;
            }

            HttpCookie warenkorb = new HttpCookie(session["user"] as string);
            warenkorb = context.Request.Cookies.Get(session["user"] as string);

            CookieStrucutre cookie = JsonConvert.DeserializeObject<CookieStrucutre>(warenkorb.Value);

            currentDict = cookie.artikel;

            foreach (var item in currentDict)
            {
                var newValue = Convert.ToInt16(nvc["anzahl" + item.Key]);
                if (newValue > 0)
                {
                    if((best.DBAnzahl(item.Key) - newValue) >= 0)
                    {
                        newDict.Add(item.Key, newValue);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            cookie.artikel = newDict;
            warenkorb.Value = JsonConvert.SerializeObject(cookie);
            context.Response.Cookies.Set(warenkorb);
            return true;
        }

        public static void ResetCookie(HttpContextBase context, HttpSessionStateBase session, NameValueCollection nvc)
        {
            if(!string.IsNullOrEmpty(session["user"] as string))
            {
                HttpCookie warenkorb = context.Request.Cookies.Get(session["user"] as string);
                CookieStrucutre cookie = JsonConvert.DeserializeObject<CookieStrucutre>(warenkorb.Value);
                warenkorb.Value = null;
                warenkorb.Expires = DateTime.MinValue;
                context.Response.Cookies.Set(warenkorb);
            }
        }
    }
}