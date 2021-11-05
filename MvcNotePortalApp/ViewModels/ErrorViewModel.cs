using MvcNotePortalApp.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcNotePortalApp.ViewModels
{
    public class ErrorViewModel : NotifyViewModelBase<ErrorMessageObject>
    {
        public ErrorViewModel()
        {
            Title = "İşlem Başarılı!";
        }
    }
}