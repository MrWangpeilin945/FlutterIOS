using System.Diagnostics;
using System.Net;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.Core.Exceptions;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Middlewares;

/// <summary>
/// 例外ハンドリングミドルウェア
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="next">次のリクエストデリゲート</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (WellshipException wex)
        {
            _logger.Log(LogLevel.Warning, wex, wex.Message);

            if (context.Response.HasStarted)
            {
                // HttpResponseの返却中に例外が出た場合はここで終了する
                return;
            }

            var errorObject = new ProblemDetails()
            {
                Type = wex.ErrorTypeUrl,
                Title = wex.ErrorTitle,
                Status = (int)wex.HttpStatusCode,
            };

            if (wex.Errors is not null && wex.Errors.Count != 0)
            {
                errorObject.Extensions.Add("errors", wex.Errors);
            }

            errorObject.Extensions.Add("traceId", Activity.Current?.Id ?? $"|{context.TraceIdentifier}");
            await WriteErrorResponseAsync(context, errorObject);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, ex.Message);

            if (context.Response.HasStarted)
            {
                // HttpResponseの返却中に例外が出た場合はここで終了する
                return;
            }

            var errorObject = new ProblemDetails()
            {
                Type = @"https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
            };
            await WriteErrorResponseAsync(context, errorObject);
        }
    }
    private async Task WriteErrorResponseAsync(HttpContext context, ProblemDetails errorDetails)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(errorDetails);
        var body = Encoding.UTF8.GetBytes(json);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)errorDetails.Status!;
        context.Response.ContentLength = body.Length;
        await context.Response.Body.WriteAsync(body);
    }
}