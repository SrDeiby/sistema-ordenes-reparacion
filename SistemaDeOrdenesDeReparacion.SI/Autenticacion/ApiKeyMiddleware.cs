using System.Data;

namespace SistemaDeOrdenesDeReparacion.SI.Autenticacion
{
    public class ApiKeyMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;


        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ConstantesDeAutenticacion.ApiKeyHeaderName, out var ApiKeyExtraida))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Api Key no proporcionada");
                return;
            }


            var apiKeyConfigurada = _configuration.GetValue<string>(ConstantesDeAutenticacion.ApiKeySectionName);
            if(!apiKeyConfigurada.Equals(ApiKeyExtraida))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Api Key no válida");
                return;
            }
            await _next(context);
        }


    }
}
