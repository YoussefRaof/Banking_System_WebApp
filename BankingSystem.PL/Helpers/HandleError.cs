using BankingSystem.PL.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankingSystem.PL.Helpers
{
    public class HandleErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Don't handle if already handled
            if (context.ExceptionHandled)
                return;

            // Get the error message
            string errorMessage = context.Exception.Message;

            // Create error model
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
                Message = errorMessage
            };

            // Set result to Error view with our model
            var result = new ViewResult
            {
                ViewName = "Error"
            };

            // Create ViewData with correct controller and properly initialized
            result.ViewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                context.ModelState)
            {
                Model = errorModel
            };

            // Set the result
            context.Result = result;

            // Mark as handled
            context.ExceptionHandled = true;
        }
    }
}
