
using System.Data;

using Moq;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class PlaceUsecasesTest
{
    [Fact]
    public async Task EC2007_会場を登録する_Repository層に値が渡ること()
    {
        // Arrange
        Mock<IPlaceRepository> mockRepo = new Mock<IPlaceRepository>();

        // テスト用の入力データ
        List<Place> inputPlaces = new List<Place>
        {
            new Place {  Code="P001", Name="Place1", InputNote=10.ToString() },
            new Place {  Code="P002", Name="Place2", InputNote=20.ToString() }
        };

        // モックの戻り値となるエラーオブジェクト
        ErrorObject dummyError = new ErrorObject { Code = "", InputNote = "", Message = ""};

        // エンティティリスト生成
        List<PlaceEntity> placeEntities = inputPlaces.Select(item =>new PlaceEntity
        {
            PlaceId = Guid.NewGuid(),
            PlaceCode = item.Code,
            Name = item.Name
        }).ToList();

        // モックのセットアップ
        mockRepo.Setup(repo => repo.UpsertPlacesAsync(It.IsAny<List<PlaceEntity>>(), It.IsAny<DateTime>(), "ExternalConnection"));

        PlaceUsecase usecases = new PlaceUsecase(mockRepo.Object);

        // Act
        List<ErrorObject> result = await usecases.StorePlacesAsync(inputPlaces);

        // Assert
        // UpsertPlacesにテスト用の入力データが渡るか、検証する。
        mockRepo.Verify(repo =>
            repo.UpsertPlacesAsync(It.Is<List<PlaceEntity>>(placeEntities =>
                placeEntities.Count == inputPlaces.Count &&
                placeEntities[0].PlaceCode == inputPlaces[0].Code &&
                placeEntities[0].Name == inputPlaces[0].Name &&
                placeEntities[1].PlaceCode == inputPlaces[1].Code &&
                placeEntities[1].Name == inputPlaces[1].Name
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
