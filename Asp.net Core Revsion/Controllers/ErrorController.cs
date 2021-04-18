using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Asp.net_Core_Revsion.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            var feature = HttpContext.Features
                    .Get<IStatusCodeReExecuteFeature>();
            var originalPath = feature.OriginalPath;
            var query = feature.OriginalQueryString;
            switch (statusCode)
            {
                case 404:
                    ViewBag.StatusCode = statusCode;
                    ViewBag.Message = $"The Page You Want does't Exist \n Path : {originalPath} \n Query : {query}";
                    break;
                case 400:
                    ViewBag.StatusCode = statusCode;
                    ViewBag.Message = "Your Request Is Bad";
                    break;
                default:
                    ViewBag.Message = "Error";
                    break;
            }
            return View();
        }

        [Route("Error")]
        public IActionResult HandleError()
        {
            var exceptionFeature = HttpContext.Features
                .Get<IExceptionHandlerFeature>();

            // same but has another property called Path
            //            var exceptionFeature1 = HttpContext.Features
            //                .Get<IExceptionHandlerPathFeature>();

            ViewBag.Source = exceptionFeature.Error.Source;
            ViewBag.Message = exceptionFeature.Error.Message;
            ViewBag.StackTrace = exceptionFeature.Error.StackTrace;

            return View();
        }
    }
}