﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcNotePortalApp.Common
{
    public static class App
    {
        public static ICommon Common = new DefaultCommon();
    }
}
