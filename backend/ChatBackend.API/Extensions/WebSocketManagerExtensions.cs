using System.Net.WebSockets;
using System.Reflection;
using ChatBackend.Handler;
using ChatBackend.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBackend.API.Extensions
{
    public static class WebSocketManagerExtensions
    {
        public static IServiceCollection AddSocketManager(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();
            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }

        public static IApplicationBuilder MapSocketManager(this IApplicationBuilder app, PathString path,
            WebSocketHandler handler)
        {
            return app.Map(path, builder => builder.UseMiddleware<SocketManagerMiddleware>(handler));
        }
    }
}