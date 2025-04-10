using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Asp.Versioning;

using Dapper;

using NLog.Web;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.TypeHandler;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Middlewares;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;
using Ryobi.Wellship.WebAPI.ResultCollector.Utilities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Logger;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.BackgroundTasks;

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

        // 認証認可サービスのDI
        builder.Services.SetupAuth();
        builder.Services.AddHttpContextAccessor();

        // 動作環境を確認してそれに合わせたサービスをDIします。
        if (true)
        {
            builder.Services.AddLocalServices()
                            .AddPostgreSqlServices();
        }
        builder.Services.AddScoped<IDbConnectionProvider, DbConnectionProvider>();
        builder.Services.AddScoped<IServiceScopeTaskRunner, ServiceScopeTaskRunner>();
        builder.Services.AddScoped<IStaffIdentityProvider, StaffIdentityFromHttpContextProvider>();
        builder.Services.AddScoped<ITenantProvider, TenantProvider>();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<ICustomLoggingService, CustomLoggingService>();

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
            options.AddDefaultPolicy(builder => builder.SetIsOriginAllowed(_ => true)
                                                       .AllowAnyMethod()
                                                       .AllowAnyHeader()
                                                       .AllowCredentials());
        });

        // カスタムタイプハンドラーの登録
        SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new SqlDateTimeOffsetTypeHandler());

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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // TODO: 負荷テストによるエラーに対応するために暫定的に設定します。
        // チューニングが必要です。

        ThreadPool.GetMinThreads(out var workMin, out var ioMin);
        ThreadPool.GetMaxThreads(out var workMax, out var ioMax);

        Console.WriteLine($"MinThreads work={workMin}, i/o={ioMin}");
        Console.WriteLine($"MaxThreads work={workMax}, i/o={ioMax}");

        ThreadPool.SetMinThreads(100, 4);

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
        services.AddScoped<IStaffLoginHistoryRepository, StaffLoginHistoryRepository>();
        services.AddScoped<IExamMenuRepository, ExamMenuRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IIntegrationRepository, IntegrationRepository>();
        services.AddScoped<IExamineeRepository, ExamineeRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<ICancelReasonRepository, CancelReasonRepository>();
        services.AddScoped<IExamItemRepository, ExamItemRepository>();
        services.AddScoped<IResultRepository, ResultRepository>();
        services.AddScoped<IAppConfigRepository, AppConfigRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IOrganizationRepository, ExternalConnection.PostgreSQL.RepositoryImpls.OrganizationRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IExamineeRepository, ExternalConnection.PostgreSQL.RepositoryImpls.ExamineeRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IAffiliationRepository, ExternalConnection.PostgreSQL.RepositoryImpls.AffiliationRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.ITeamRepository, ExternalConnection.PostgreSQL.RepositoryImpls.TeamRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IPlaceRepository, ExternalConnection.PostgreSQL.RepositoryImpls.PlaceRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IThresholdRepository, ExternalConnection.PostgreSQL.RepositoryImpls.ThresholdRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IConsultRepository, ExternalConnection.PostgreSQL.RepositoryImpls.ConsultRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.ITicketRepository, ExternalConnection.PostgreSQL.RepositoryImpls.TicketRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IPlaceScheduleRepository, ExternalConnection.PostgreSQL.RepositoryImpls.PlaceScheduleRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IStaffRepository, ExternalConnection.PostgreSQL.RepositoryImpls.StaffRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IExamNormalValueRangeRepository, ExternalConnection.PostgreSQL.RepositoryImpls.ExamNormalValueRangeRepository>();
        services.AddScoped<ExternalConnection.PostgreSQL.RepositoryImpls.IExternalExamItemDetailsRepository, ExternalConnection.PostgreSQL.RepositoryImpls.ExternalExamItemDetailsRepository>();
        return services;
    }
    /// <summary>
    /// ユースケースを追加します。
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationUsecase, AuthenticationUsecase>();
        services.AddScoped<IPlaceScheduleUsecase, PlaceScheduleUsecase>();
        services.AddScoped<IConsultUsecase, ConsultUsecase>();
        services.AddScoped<IHomeMenuUsecase, HomeMenuUsecase>();
        services.AddScoped<IStaffUsecase, StaffUsecase>();
        services.AddScoped<IExamMenuUsecase, ExamMenuUsecase>();
        services.AddScoped<IEquipmentUsecase, EquipmentUsecase>();
        services.AddScoped<IIntegrationUsecase, IntegrationUsecase>();
        services.AddScoped<IProgressUsecase, ProgressUsecase>();
        services.AddScoped<ICancelReasonUsecase, CancelReasonUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IOrganizationUsecase, ExternalConnection.Usecases.Default.OrganizationUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IExamineeUsecase, ExternalConnection.Usecases.Default.ExamineeUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.ITeamUsecase, ExternalConnection.Usecases.Default.TeamUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IPlaceUsecase, ExternalConnection.Usecases.Default.PlaceUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IThresholdUsecase, ExternalConnection.Usecases.Default.ThresholdUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IConsultUsecase, ExternalConnection.Usecases.Default.ConsultUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.ITicketUsecase, ExternalConnection.Usecases.Default.TicketUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IPlaceScheduleUsecase, ExternalConnection.Usecases.Default.PlaceScheduleUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IStaffUsecase, ExternalConnection.Usecases.Default.StaffUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IExamNormalValueRangeUsecase, ExternalConnection.Usecases.Default.ExamNormalValueRangeUsecase>();
        services.AddScoped<ExternalConnection.Usecases.Default.IDataImportUsecase, ExternalConnection.Usecases.KitashinagawaClinic.DataImportUsecase>();

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

    /// <summary>
    /// 認証・認可サービスの設定
    /// </summary>
    public static IServiceCollection SetupAuth(this IServiceCollection services)
    {
        // 認証・認可で使用する設定
        // TODO: 設定の場所が決まるまでの仮置きです。設定ができ次第移植すること。
        var authSettings = new AuthSettings()
        {
            AccessTokenLifetime = TimeSpan.FromMinutes(3),
            RefreshTokenLifeTime = TimeSpan.FromHours(12),
            SecretKey = "secret key length required 128 bit"
        };
        services.AddSingleton(x => authSettings);
        services.AddScoped<IAuthService, AuthService>();

        // NOTE: Program.csに記述するとゴチャつくのでAddJwtBearerに渡すデリゲートは別ファイルに分けた方が良いかも
        services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
                {
                    // JWTのクレーム名を自動でマッピングしない設定です。
                    x.MapInboundClaims = false;
                    x.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Sub;
                    x.TokenValidationParameters.RoleClaimType = CustomClaimTypes.Role;
                    // NOTE: Issはアクセスされたホスト名を使用したいためカスタム検証で検証します
                    x.TokenValidationParameters.ValidateIssuer = false;
                    // NOTE: Audはアクセスされたホスト名を使用したいためカスタム検証で検証します
                    x.TokenValidationParameters.ValidateAudience = false;
                    // 署名検証設定
                    // ・トークンの改ざんを検出することで、不正に書き換えられたJWTを拒否します
                    x.TokenValidationParameters.IssuerSigningKey = authSettings.JwtSigningKey;
                    x.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    // jwtの時刻検証
                    // ・トークンの有効開始時間（nbf）と有効期限（exp）を検証して無効なJWTを拒否します
                    // NOTE: クライアント側にNTPがない可能性もあるので要注意。
                    //       iatでサーバーとクライアントの時刻ズレは検出できるが通信遅延なども考えると一定の猶予があった方がよい？
                    x.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(3);
                    x.TokenValidationParameters.ValidateLifetime = true;

                    x.Events = new JwtBearerEvents()
                    {
                        // JWT検証前にcontext.Tokenを確認し、存在しない場合にはcookieの"authorization"の値をjwtとして使用します。
                        // 優先順位が HttpHeader > cookie になっています。
                        OnMessageReceived = context =>
                        {
                            if (string.IsNullOrEmpty(context.Token))
                            {
                                var token = context.HttpContext.Request.Cookies["authorization"];
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        },
                        // アクセスされたホスト名がClaimのissと等しい・audに含まれているかどうか追加検証します
                        // 自分自身が発行したトークンのみを受け付けるための検証と、
                        // マルチテナント環境において異なるテナントへのアクセスを拒否するための検証です
                        OnTokenValidated = context =>
                        {
                            var host = context.HttpContext.Request.Host.Value;
                            // JWTを発行した主体のチェックをします。
                            // 現在は自分自身が発行したものであることを要求しています。
                            var iss = context.Principal?.FindFirst(JwtRegisteredClaimNames.Iss)?.Value;
                            if (iss is null || !host.Equals(iss, StringComparison.OrdinalIgnoreCase))
                            {
                                context.Fail("Invalid token issuer.");
                            }
                            // JWTの使用対象のチェックをします。
                            // 自分自身に向けて発行されたものであることを要求しています。
                            var auds = context.Principal?.FindAll(JwtRegisteredClaimNames.Aud)?.Select(x => x.Value);
                            if (auds is null || !auds.Any(x => host.Equals(x, StringComparison.OrdinalIgnoreCase)))
                            {
                                context.Fail("Invalid token audience.");
                            }
                            // subがJWTに含まれ、GuidにParseできることをチェックします。
                            // ログイン状態のとき操作者を識別するために使用する情報のため必須としています。
                            var sub = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                            if (!Guid.TryParse(sub, out _))
                            {
                                context.Fail("Invalid token sub.");
                            }
                            // unique_nameがJWTに含まれることをチェックします。
                            var uniqueName = context.Principal?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
                            if (string.IsNullOrEmpty(uniqueName))
                            {
                                context.Fail("Invalid token unique_name.");
                            }
                            // roleがJWTに含まれ、有効なロール名であることをチェックします。
                            var role = context.Principal?.FindFirst(CustomClaimTypes.Role)?.Value;
                            if (!Enum.TryParse<Role>(role, out _))
                            {
                                context.Fail("Invalid token role.");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        return services;
    }
}
