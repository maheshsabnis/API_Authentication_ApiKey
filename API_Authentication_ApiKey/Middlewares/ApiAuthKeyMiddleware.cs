using API_Authentication_ApiKey.Services;
using System.Net;

namespace API_Authentication_ApiKey.Middlewares
{
    public record ErrorMessage
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
    public class ApiAuthKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IApiAuthKeyValidatorService _service;

        /// <summary>
        /// Inject the Required Dependencies
        /// </summary>
        public ApiAuthKeyMiddleware(RequestDelegate next, IConfiguration configuration, IApiAuthKeyValidatorService service)
        {
            _next = next;
            _configuration = configuration;
            _service = service;
        }
        /// <summary>
        /// Method to validate the ApiKey
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Read the Header NAme from the appsettings.json

                string? headerName = _configuration.GetValue<string>("ApiAuthHeader");
                if (string.IsNullOrEmpty(context.Request.Headers[headerName]))
                    throw new Exception("Missing Header Values on the server");

                // Make sure that the ApiAuthHeader is  present in the HTTP Request Header 
                if (string.IsNullOrEmpty(context.Request.Headers[headerName]))
                    throw new Exception("Missing Header Values in the request");

                // Read the Key

                string? apiAuthKey = context.Request.Headers[headerName];

                // Validate the Key 

                _service.ValidateApiAuthKey(apiAuthKey, out bool isValid);
                // Return the UnAuthorized Response
                if (!isValid)
                { 
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    ErrorMessage errorMessage = new ErrorMessage()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "The supplied Authorization Information is invalid",
                    };
                    await context.Response.WriteAsJsonAsync(errorMessage);
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            { 
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ErrorMessage errorMessage = new ErrorMessage()
                {
                     StatusCode = context.Response.StatusCode,
                     Message = ex.Message,
                };

                await context.Response.WriteAsJsonAsync(errorMessage);

            }
        }
    }


    // The Middleware class

    public static class ApiAuthKeyMiddlewareExtension
    {
        public static void UseApiKeyAuthorization(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ApiAuthKeyMiddleware>();
        }
    }
}
