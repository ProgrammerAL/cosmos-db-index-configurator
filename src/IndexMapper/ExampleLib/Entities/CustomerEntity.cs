using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;

namespace ProgrammerAl.CosmosDbIndexConfigurator.ExampleLib.Entities;

[IdConfiguredEntity(containerName: "Customers")]
public class CustomerEntity
{
    [IncludePartitionKeyAttribute]
    [IncludeIndex]
    public required Guid EntityId { get; set; }

    [IncludeIndex]
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required int BirthYear { get; set; }

    public required List<OrderSubEntity> OrdersList { get; set; }
    public required OrderSubEntity[] OrdersArray { get; set; }

    public class OrderSubEntity
    {
        [IncludeIndex]
        public required Guid EntityId { get; set; }

        [IncludeIndex]
        public required Guid OrderNumber { get; set; }

        public required string ProductName { get; set; }

        public required ProductSummarySubEntity ProductSummary { get; set; }
    }

    public class ProductSummarySubEntity
    {
        [IncludeIndex]
        public required Guid EntityId { get; set; }

        [IncludeIndex]
        public required string Name { get; set; }

        public required DateTime PurchaseDateTimeUtc { get; set; }

        public ProductReviewOrderSubEntity? Review { get; set; }

        public class ProductReviewOrderSubEntity
        {
            [IncludeIndex]
            public required Guid EntityId { get; set; }

            public required int Stars { get; set; }
            public string? Comment { get; set; }
        }
    }
}
