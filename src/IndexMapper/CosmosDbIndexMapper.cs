using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using ProgrammerAl.SourceGenerators.PublicInterfaceGenerator.Attributes;

using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;
using ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.PropertyMappers;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;

public record CosmosDbIndexMap(ImmutableArray<MappedIndexes> Indexes, ImmutableArray<AttributeLoadError> LoadErrors);
public record AttributeLoadError(string TypeErroredOn, Exception Exception);

[GenerateInterface]
public class CosmosDbIndexMapper : ICosmosDbIndexMapper
{
    private readonly IndexPropertyMapper _indexMapper = new IndexPropertyMapper();
    private readonly PartitionKeyPropertyMapper _partitionKeyMapper = new PartitionKeyPropertyMapper();

    /// <param name="assembly">Assembly to load from</param>
    public CosmosDbIndexMap MapIndexes(Assembly assembly)
    {
        var (typesWithIdsToMap, loadErrors) = LoadClassesWithIdsToMap(assembly);
        var mappedIndexes = LoadMappedIndexesFromDbSetProperties(typesWithIdsToMap);

        return new CosmosDbIndexMap(mappedIndexes, loadErrors);
    }

    private ImmutableArray<MappedIndexes> LoadMappedIndexesFromDbSetProperties(ImmutableArray<MappedType> typesToMap)
    {
        var builder = ImmutableArray.CreateBuilder<MappedIndexes>(typesToMap.Length);

        foreach (var typeToMap in typesToMap)
        {
            var partitionKey = _partitionKeyMapper.MapPropertyWithAttribute(typeToMap.Type, indexPath: "/");
            var indexes = _indexMapper.MapPropertiesWithAttribute(typeToMap.Type, indexPath: "/");

            var mappedIndexes = new MappedIndexes(ContainerName: typeToMap.ContainerName, partitionKey, indexes);
            builder.Add(mappedIndexes);
        }

        return builder.MoveToImmutable();
    }

    private static (ImmutableArray<MappedType>, ImmutableArray<AttributeLoadError>) LoadClassesWithIdsToMap(Assembly assembly)
    {
        var mappedTypesBuilder = ImmutableArray.CreateBuilder<MappedType>();
        var errorsBuilder = ImmutableArray.CreateBuilder<AttributeLoadError>();
        foreach (var type in assembly.ExportedTypes)
        {
            try
            {
                var attr = type.GetCustomAttribute<IdConfiguredEntityAttribute>();
                if (attr is object)
                {
                    mappedTypesBuilder.Add(new MappedType(type, attr.ContainerName));
                }
            }
            catch (Exception ex)
            {
                errorsBuilder.Add(new AttributeLoadError(type.FullName, ex));
            }
        }

        return (mappedTypesBuilder.ToImmutableArray(), errorsBuilder.ToImmutableArray());
    }

    private record MappedType(Type Type, string ContainerName);
}
