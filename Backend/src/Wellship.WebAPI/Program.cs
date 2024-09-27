using Microsoft.AspNetCore.Mvc;

using NLog.Web;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Middlewares;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;
using Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

namespace Ryobi.Wellship.WebAPI;

/// <summary>
/// メインのクラス
/// </summary>
public class Program
{
    /// <summary>
    /// エントリーポイントです。
    /// </summary>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });

        builder.Services.AddHealthChecks()
                        .AddCheck<HealthCheck>("database");
        builder.Services.AddScoped<PostgresConnector>();
        builder.Services.AddRepositories();
        builder.Services.AddUseCases();

        builder.Services.AddOpenApiDocument(options =>
        {
            options.PostProcess = document =>
            {
                document.Info = new NSwag.OpenApiInfo
                {
                    Version = "v1",
                    Title = "WELLSHIP",
                    Description = "WELLSHIPのバックエンドです。"
                };
            };
        });

        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi();
            app.UseReDoc(options =>
            {
                options.Path = "/redoc";
            });
        }

        app.MapHealthChecks("/healthz");
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

/// <summary>
/// ServiceCollectionを拡張します。
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    /// リポジトリを追加します。
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IHealthCheckRepository, HealthCheckRepository>();
        services.AddScoped<IConsultRepository, ConsultRepository>();
        services.AddScoped<IPlaceScheduleRepository, PlaceScheduleRepository>();
        return services;
    }
    /// <summary>
    /// ユースケースを追加します。
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IPlaceScheduleUsecase, PlaceScheduleUsecase>();
        services.AddScoped<IConsultUsecase, ConsultUsecase>();
        return services;
    }
}
