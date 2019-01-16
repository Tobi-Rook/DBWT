using System.Web.Mvc;
using System.Collections.Specialized;

using DBWT.Models;

namespace DBWT.Controllers
{
    public class DetailController : Controller
    {
        // GET: Detail
        public ActionResult Index(string ID)
        {
            NameValueCollection nvc = Request.Form;

            Detail detail = new Detail();

            if (!int.TryParse(ID, out int intId))
            {
                detail.gueltig = false;
            }

            Login log = new Login();
            LoginManagement.Login(Session, nvc, log);
            
            detail.ID = intId;
            detail.log = log;
            detail.Details();
            detail.Zutaten();

            ViewBag.Anzahl = CookieManagement.GetAnzahl(HttpContext, Session);
            if(CookieManagement.AddToCookie(HttpContext, Session, nvc))
            {
                detail.UserMessage = "Die Mahlzeit \"" + detail.pro.name + "\" wurde dem Warenkorb hinzugefügt.";
                detail.UserMessageStatus = "true";
            }
            else
            {
                detail.UserMessage = "Ihr Warenkorb enthält bereits die maximale Bestellmenge der Mahlzeit \"" + detail.pro.name + "\".";
                detail.UserMessageStatus = "false";
            }
            return View(detail);
        }
    }
}