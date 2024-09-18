using NLog.Web;

using Ryobi.WELLSHIP.WebAPI.ResultCollector.Infrastructure.PostgreSQL;
using Ryobi.WELLSHIP.WebAPI.ResultCollector.Usecases;

namespace Ryobi.WELLSHIP.WebAPI;

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
        return services;
    }
    /// <summary>
    /// ユースケースを追加します。
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IPlaceScheduleUsecase, PlaceScheduleUsecase>();
        return services;
    }
}
