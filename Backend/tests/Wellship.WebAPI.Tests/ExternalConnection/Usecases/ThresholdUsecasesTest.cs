
using System.Data;

using Moq;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class ThresholdUsecasesTest
{
    [Fact]
    public async Task EC2014_基準パターンを登録する_Repository層に値が渡ること()
    {
        // Arrange
        Mock<IThresholdRepository> mockRepo = new Mock<IThresholdRepository>();

        // テスト用の入力データ
        List<Threshold> inputThresholds = new List<Threshold>
        {
            new Threshold { Code="T001", Name="Threshold1", InputNote=10.ToString() },
            new Threshold { Code="T002", Name="Threshold2", InputNote=20.ToString() }
        };

        // モックの戻り値となるエラーオブジェクト
        ErrorObject dummyError = new ErrorObject { Code = "", InputNote = "", Message = ""};

        // エンティティリスト生成
        List<ThresholdEntity> thresholdEntities = inputThresholds.Select(item =>new ThresholdEntity
        {
            ThresholdId = Guid.NewGuid(),
            ThresholdCode = item.Code,
            Name = item.Name
        }).ToList();

        // モックのセットアップ
        mockRepo.Setup(repo => repo.UpsertThresholdsAsync(It.IsAny<List<ThresholdEntity>>(), It.IsAny<DateTime>(), "ExternalConnection"));

        ThresholdUsecase usecases = new ThresholdUsecase(mockRepo.Object, TimeProvider.System);

        // Act
        List<ErrorObject> result = await usecases.StoreThresholdsAsync(inputThresholds);

        // Assert
        // UpsertThresholdsにテスト用の入力データが渡るか、検証する。
        mockRepo.Verify(repo =>
            repo.UpsertThresholdsAsync(It.Is<List<ThresholdEntity>>(thresholdEntities =>
                thresholdEntities.Count == inputThresholds.Count &&
                thresholdEntities[0].ThresholdCode == inputThresholds[0].Code &&
                thresholdEntities[0].Name == inputThresholds[0].Name &&
                thresholdEntities[1].ThresholdCode == inputThresholds[1].Code &&
                thresholdEntities[1].Name == inputThresholds[1].Name
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
