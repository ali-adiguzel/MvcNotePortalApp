using MvcNotePortalApp.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcNotePortalApp.BusinessLayer.Result
{
    /// <summary>
    /// Hatalarımız için List<string> tipinde bir liste property'si, sonuçlar için ise T tipinde bir property içeren generic kendi sınıfımız. Bu sayede business işlemlerinde hata varsa hataları alacağız, yoksa sonucu.
    /// </summary>
    /// <typeparam name="T">Hata veya sonuçların hangi model için döneceği. <b>(Entities)</b></typeparam>

    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMessageObject> Errors { get; set; }
        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessageObject>();
        }

        public void AddError(ErrorMessage errorCode, string message)
        {
            Errors.Add(new ErrorMessageObject() { Code = errorCode, Message = message });
        }
    }
}
