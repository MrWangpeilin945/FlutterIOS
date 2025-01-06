using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using FluentAssertions;

using Moq;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases
{
    public class OrganizationUsecasesTests
    {

        [Fact]
        public async Task 団体を登録する時エラーがないこと()
        {
            // Arrange
            var organizationRepositoryMock = new Mock<IOrganizationRepository>();
            var organizations = new List<Organization>
            {
                new Organization { Code = "1", Name = "団体1", InputNote = "1" },
                new Organization { Code = "2", Name = "団体2", InputNote = "2" }
            };
            var organizationEntities = organizations.Select(item => new OrganizationEntity
            {
                OrganizationCode = item.Code,
                Name = item.Name
            }).ToList();

            organizationRepositoryMock.Setup(r => r.UpsertOrganizationsAsync(organizationEntities, DateTime.Now, "ExternalConnection"));

            var usecases = new OrganizationUsecases(organizationRepositoryMock.Object);

            // Act & Assert
            await usecases.Invoking(x => x.StoreOrganizationsAsync(organizations))
                                .Should().NotThrowAsync();
        }
    }
}
