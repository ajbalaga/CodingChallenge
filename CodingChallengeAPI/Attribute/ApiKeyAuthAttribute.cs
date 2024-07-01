using CodingChallenge.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodingChallengeAPI.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiKeyAuthAttribute : ActionFilterAttribute
    {
        private const string ApiKeyHeaderName = "X-Api-Key";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiKeyAuthAttribute>>();
            var apiKeyService = context.HttpContext.RequestServices.GetRequiredService<IApiKeyService>();

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                logger.LogWarning("API key was not provided.");
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!apiKeyService.ValidateApiKey(potentialApiKey))
            {
                logger.LogWarning("Invalid API key provided.");
                context.Result = new UnauthorizedResult();
                return;
            }

            logger.LogInformation("API key validated successfully.");
            base.OnActionExecuting(context);
        }
    }
}
