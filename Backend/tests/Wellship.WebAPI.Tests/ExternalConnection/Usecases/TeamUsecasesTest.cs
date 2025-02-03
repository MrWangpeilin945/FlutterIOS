
using System.Data;

using Moq;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class TeamUsecasesTest
{
    [Fact]
    public async Task EC2006_班を登録する_Repository層に値が渡ること()
    {
        // Arrange
        Mock<ITeamRepository> mockRepo = new Mock<ITeamRepository>();

        // テスト用の入力データ
        List<Team> inputTeams = new List<Team>
        {
            new Team { Code="T001", Name="Team1", InputNote=10.ToString() },
            new Team { Code="T002", Name="Team2", InputNote=20.ToString() }
        };

        // モックの戻り値となるエラーオブジェクト
        ErrorObject dummyError = new ErrorObject { Code = "", InputNote = "", Message = ""};

        // エンティティリスト生成
        List<TeamEntity> teamEntities = inputTeams.Select(item =>new TeamEntity
        {
            TeamId = Guid.NewGuid(),
            TeamCode = item.Code,
            Name = item.Name
        }).ToList();

        // モックのセットアップ
        mockRepo.Setup(repo => repo.UpsertTeamsAsync(It.IsAny<List<TeamEntity>>(), It.IsAny<DateTime>(), "ExternalConnection"));

        TeamUsecase usecases = new TeamUsecase(mockRepo.Object);

        // Act
        List<ErrorObject> result = await usecases.StoreTeamsAsync(inputTeams);

        // Assert
        // UpsertTeamsにテスト用の入力データが渡るか、検証する。
        mockRepo.Verify(repo =>
            repo.UpsertTeamsAsync(It.Is<List<TeamEntity>>(teamEntities =>
                teamEntities.Count == inputTeams.Count &&
                teamEntities[0].TeamCode == inputTeams[0].Code &&
                teamEntities[0].Name == inputTeams[0].Name &&
                teamEntities[1].TeamCode == inputTeams[1].Code &&
                teamEntities[1].Name == inputTeams[1].Name
            ), It.IsAny<DateTime>(), "ExternalConnection"), Times.Once);

        // 戻り値のエラーリストについて
        // 正常系の想定であり、エラーオブジェクトの中身が空っぽのものだけであることを確認する。
        Assert.True(CountError(result) == 0);
        
    }
    

    private int CountError(List<ErrorObject> errorList)
    {
        if(errorList.Count <= 0) {
            return 0;
        }
        int errorCount = 0;
        foreach(ErrorObject error in errorList) {
            if(typeof(ErrorObject) == error.GetType()) { 
                if( !String.IsNullOrEmpty(error.Code) ||
                    !String.IsNullOrEmpty(error.Message) ||
                    !String.IsNullOrEmpty(error.InputNote) )
                {
                    errorCount++;
                }
            }
        }
        return errorCount;
    }
}
