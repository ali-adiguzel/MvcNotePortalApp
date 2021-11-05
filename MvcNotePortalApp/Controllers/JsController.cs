using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcNotePortalApp.Controllers
{
    public class JsController : Controller
    {
        // GET: Js
        public JavaScriptResult MostLikeds()
        {
            string js = "$(function () { $('#mostLikeBtn').addClass('active'); });";

            return JavaScript(js);
        }

        public JavaScriptResult LastNotes()
        {
            string js = "$(function () { $('#lastNotesBtn').addClass('active'); });";

            return JavaScript(js);
        }

        public JavaScriptResult MyNotes()
        {
            string js = "$(function () { $('#navbarDarkDropdownMenuLink').addClass('active'); });";

            return JavaScript(js);
        }

        public JavaScriptResult About()
        {
            string js = "$(function () { $('#aboutBtn').addClass('active'); });";

            return JavaScript(js);
        }

        public JavaScriptResult Login()
        {
            string js = "$(function () { $('#mostLikeBtn').addClass('active'); });";

            return JavaScript(js);
        }
    }
}