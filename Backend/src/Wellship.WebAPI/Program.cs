using Microsoft.AspNetCore.Mvc;

using NLog.Web;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
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
                        .AddCheck<HealthCheck>("app");
        builder.Services.AddRepositories();
        builder.Services.AddUseCases();

        // 動作環境を確認してそれに合わせたサービスをDIします。
        if (true)
        {
            builder.Services.AddLocalServices()
                            .AddPostgreSqlServices();
        }
        builder.Services.AddScoped<IDbConnectionProvider, DbConnectionProvider>();

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

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                                                       .AllowAnyMethod()
                                                       .AllowAnyHeader());
        });

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
            app.UseCors();
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
        services.AddScoped<IHomeMenuRepository, HomeMenuRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IExamMenuRepository, ExamMenuRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IIntegrationRepository, IntegrationRepository>();
        services.AddScoped<IExamineeRepository, ExamineeRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<ICancelReasonRepository, CancelReasonRepository>();
        services.AddScoped<IExamItemRepository, ExamItemRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IOrganizationRepository, ExternalConnection.PostgreSQL.RepositoryImpls.OrganizationRepository>();
        return services;
    }
    /// <summary>
    /// ユースケースを追加します。
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IPlaceScheduleUsecase, PlaceScheduleUsecase>();
        services.AddScoped<IConsultUsecase, ConsultUsecase>();
        services.AddScoped<IHomeMenuUsecase, HomeMenuUsecase>();
        services.AddScoped<IStaffUsecase, StaffUsecase>();
        services.AddScoped<IExamMenuUsecase, ExamMenuUsecase>();
        services.AddScoped<IEquipmentUsecase, EquipmentUsecase>();
        services.AddScoped<IIntegrationUsecase, IntegrationUsecase>();
        services.AddScoped<IProgressUsecase, ProgressUsecase>();
        services.AddScoped<ICancelReasonUsecase, CancelReasonUsecase>();
        services.AddScoped<ExternalConnection.Usecases.IOrganizationUsecases, ExternalConnection.Usecases.OrganizationUsecases>();

        return services;
    }
    /// <summary>
    /// ローカル環境で動作させる場合のみ使用するサービス群
    /// </summary>
    public static IServiceCollection AddLocalServices(this IServiceCollection services)
    {
        // 接続文字列はアプリケーション全体の寿命で管理したいためSingletonでDIする
        services.AddSingleton<IConnectionStringProvider, EnvironmentVariableConnectionStringProvider>();
        return services;
    }
    /// <summary>
    /// AWS環境で動作させる場合のみ使用するサービス群
    /// </summary>
    public static IServiceCollection AddAwsServices(this IServiceCollection services)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// PostgreSQLに接続する場合のみ使用するサービス群
    /// </summary>
    public static IServiceCollection AddPostgreSqlServices(this IServiceCollection services)
    {
        // データソースはアプリケーション全体の寿命で管理したいためSingletonでDIする
        services.AddSingleton<IDbDataSourceRegistry, NpgsqlDbDataSourceRegistry>();
        return services;
    }
}
