using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.Silanis
{
    public abstract class BaseApiController : ApiController
    {
        protected abstract string ApiRoot { get; }

        [DebuggerStepThrough]
        protected string Format(string format = null, params object[] @params)
        {
            var builder = new StringBuilder(ApiRoot);

            if (!object.Equals(format, null))
                builder.AppendFormat(format, @params);

            return builder.ToString();
        }

        protected HttpResponseMessage HttpProxy(HttpRequestMessage request, string method)
        {
            var message = new StringBuilder();

#if PdfForge
            message.AppendLine("To use this feature, please update to the newest version of PDF Architect 5.1.<br/>"); //en
            message.AppendLine("Pour utiliser cette fonctionnalité, veuillez svp mettre votre logiciel à jour avec la nouvelle version 5.1.<br/>"); //fr
            message.AppendLine("Чтобы использовать эту функцию, пожалуйста, обновитесь к более новой версии PDF Architect 5.1.<br/>"); //ru
            message.AppendLine("Um die Funktion wieder nutzen zu können müssen Sie auf die neuste Version von PDF Architect 5.1 updaten.<br/>"); //de
#endif

#if !LuluSoft
            message.AppendLine("To use this feature, please update to the newest version of Soda PDF 9.3.<br/>"); //en
            message.AppendLine("Pour utiliser cette fonctionnalité, veuillez svp mettre votre logiciel à jour avec la nouvelle version 9.3.<br/>"); //fr
            message.AppendLine("Чтобы использовать эту функцию, пожалуйста, обновитесь к более новой версии Soda PDF 9.3.<br/>"); //ru
            message.AppendLine("Um die Funktion wieder nutzen zu können müssen Sie auf die neuste Version von Soda PDF 9.3 updaten.<br/>"); //de
#endif

            return request.CreateResponse(HttpStatusCode.BadRequest, new
            {
                error = "invalid_request",
                error_description = message.ToString()
            });
        }
    }
}
