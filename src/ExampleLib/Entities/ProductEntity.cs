
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;

namespace ProgrammerAl.CosmosDbIndexConfigurator.ExampleLib.Entities;

[IdConfiguredEntity(containerName: "Products")]
public class ProductEntity
{
    [IncludeIndex]
    public required Guid EntityId { get; set; }

    [IncludeIndex]
    public required string ProductName { get; set; }

    [IncludeIndex]
    public required bool IsEnabled { get; set; }

    [IncludeIndex]
    public required int NonDecimalPrice { get; set; }

    [IncludeIndex]
    public required decimal DisplayPrice { get; set; }

    [IncludeIndex]
    public required DateTime CreateDateTime { get; set; }

    [IncludeIndex]
    public required float PopularityRating { get; set; }

    [IncludeIndex]
    public required short DisplayOrder { get; set; }

    [IncludeIndex]
    public required long UserPoints { get; set; }
}
