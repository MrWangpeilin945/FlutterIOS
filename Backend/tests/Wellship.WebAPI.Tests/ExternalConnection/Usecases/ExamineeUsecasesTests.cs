using System.Data;
using System.Data.Common;

using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases
{
    public class ExamineeUsecasesTests
    {
        private readonly ExamineeUsecase _usecases;

        public ExamineeUsecasesTests()
        {
            // テスト用依存関係を注入します
            var dbConnectionProvider = new Mock<IDbConnectionProvider>();
            var organizationRepository = new Mock<IOrganizationRepository>();
            var examineeRepository = new Mock<IExamineeRepository>();
            var affiliationRepository = new Mock<IAffiliationRepository>();

            dbConnectionProvider.Setup(x => x.GetOrOpenAsync()).ReturnsAsync(new Mock<DbConnection>().Object);
            organizationRepository.Setup(x => x.GetOrganizationsByCodesAsync(It.IsAny<List<string>>())).ReturnsAsync(new List<string> { "001" });
            examineeRepository.Setup(x => x.UpsertExamineesAsync(It.IsAny<List<ExamineeEntity>>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            affiliationRepository.Setup(x => x.InsertAffiliationsAsync(It.IsAny<List<Examinee>>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _usecases = new ExamineeUsecase(dbConnectionProvider.Object, organizationRepository.Object, examineeRepository.Object, affiliationRepository.Object);
        }

        [Fact]
        public async Task 正常に受診者を登録する()
        {
            // Arrange
            var examinees = new List<Examinee>
            {
                new() { ExamineeCode = "001", Name = "Test Taro", KanaName = "テスト タロウ", Sex = Sex.男, Birthdate = new DateTime(1990, 1, 1), Affiliations = [new() { OrganizationCode = "001" }], InputNote = "001" }
            };

            // Act
            var result = await _usecases.StoreExamineesAsync(examinees);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task 存在しない団体コードが含まれる場合エラーを返す()
        {
            // Arrange
            var examinees = new List<Examinee>
            {
                new Examinee { ExamineeCode = "002", Name = "Test Hanako", KanaName = "テスト ハナコ", Sex = Sex.女, Birthdate = new DateTime(1991, 2, 2), Affiliations = [new() { OrganizationCode = "999" }], InputNote = "002" }
            };

            var expectedErrors = new List<ErrorObject>
            {
                new() {
                    Code = "10001",
                    Message = "指定されたOrganizationCodeがシステム上に存在しません。Code:999",
                    InputNote = "002"
                }
            };

            // Act
            var result = await _usecases.StoreExamineesAsync(examinees);

            // Assert
            result.Should().BeEquivalentTo(expectedErrors);
        }
    }
}
