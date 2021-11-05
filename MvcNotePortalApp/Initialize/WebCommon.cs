using MvcNotePortalApp.Common;
using MvcNotePortalApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcNotePortalApp.Initialize
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            if (HttpContext.Current.Session["login"] != null)
            {
                NotePortalUser user = HttpContext.Current.Session["login"] as NotePortalUser;

                return user.UserName;
            }

            return "NPSystem";
        }
    }
}