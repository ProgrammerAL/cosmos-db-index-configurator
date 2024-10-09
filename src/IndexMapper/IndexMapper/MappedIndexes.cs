using System.Collections.Immutable;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;

public record MappedIndexes(string ContainerName, string? PartitionKey, ImmutableArray<string> IncludedIndexes);
