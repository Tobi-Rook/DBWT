using System.Collections.Specialized;
using System.Web;

using DBWT.Models;

namespace DBWT
{
    public class LoginManagement
    {
        public static void Login(HttpSessionStateBase session, NameValueCollection nvc, Login log)
        {
            log.signout = !string.IsNullOrEmpty(nvc["signout"] as string);
            if (!string.IsNullOrEmpty(session["user"] as string) && !string.IsNullOrEmpty(session["role"] as string) && !log.signout)
            {
                log.signedIn = true;
                log.user = session["user"] as string;
                log.role = session["role"] as string;
            }
            else if (log.signout)
            {
                log.signedIn = false;
                session["user"] = "";
                session["role"] = "";
                log.role = "Gast";
            }
            else
            {
                log.signedIn = false;
                log.password = nvc["password"];
                log.user = nvc["user"];
                log.Password();
                if (log.signedIn)
                {
                    session["user"] = log.user;
                    session["role"] = log.role;
                }
                else
                {
                    log.role = "Gast";
                }
            }
        }
    }
}