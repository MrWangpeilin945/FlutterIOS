namespace Ryobi.Wellship.WebAPI.DapperSample.Controllers.V1;

using Microsoft.AspNetCore.Mvc;

using Wellship.WebAPI.DapperSample.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// テーブルの一覧を取得するコントローラーです。サンプルのためController=UseCaseのレベルとし、
/// Repositoryを呼び出しています
/// </summary>
/// <param name="pgUserRepository">PgUserのリポジトリ</param>
[ApiController]
[ApiVersion("1")]
public class TablesController(IPgUserRepository pgUserRepository) : ControllerBase
{
    private readonly IPgUserRepository _pgUserRepository = pgUserRepository;

    /// <summary>
    ///
    /// </summary>
    [HttpGet]
    [Route("api/v{version:apiVersion}/sample/tables")]
    public async Task<IActionResult> GetTablesAsync()
    {
        var users = await _pgUserRepository.GetAllUsersAsync();
        return Ok(users);
    }
}