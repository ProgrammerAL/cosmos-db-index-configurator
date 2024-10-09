
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;

namespace ProgrammerAl.CosmosDbIndexConfigurator.ExampleLib.Entities;

[IdConfiguredEntity(containerName: "Orders")]
public record OrderEntity([property: IncludePartitionKey][property: IncludeIndex] Guid EntityId, [property: IncludeIndex] string ProductName);

