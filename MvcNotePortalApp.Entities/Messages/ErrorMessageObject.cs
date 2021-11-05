using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcNotePortalApp.Entities.Messages
{
    public class ErrorMessageObject
    {
        public ErrorMessage Code { get; set; }
        public string Message { get; set; }
    }
}
