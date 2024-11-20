using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using ProgrammerAl.SourceGenerators.PublicInterfaceGenerator.Attributes;

using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;
using ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.PropertyMappers;
using System.IO;
using System.Runtime.InteropServices;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;

public record CosmosDbIndexMap(ImmutableArray<MappedIndexes> Indexes, ImmutableArray<AttributeLoadError> LoadErrors);
public record AttributeLoadError(string TypeErroredOn, Exception Exception);

[GenerateInterface]
public class CosmosDbIndexMapper : ICosmosDbIndexMapper
{
    private readonly IndexPropertyMapper _indexMapper = new IndexPropertyMapper();
    private readonly PartitionKeyPropertyMapper _partitionKeyMapper = new PartitionKeyPropertyMapper();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assemblyResolverPaths">Collection of paths that hold dll files to resolve from when loading assembly info. Note: The paths are to the directory that store dlls, not paths to individual dll files.</param>
    /// <param name="assemblyPath">Path to the assembly file to interrogate</param>
    public CosmosDbIndexMap MapIndexes(IEnumerable<string> assemblyResolverPaths, string assemblyPath)
    {
        var assembliesToResolve = new List<string>();
        foreach (var path in assemblyResolverPaths)
        {
            var dirDlls = Directory.GetFiles(path, "*.dll");
            assembliesToResolve.AddRange(dirDlls);
        }

        var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
        var runtimeDirDlls = Directory.GetFiles(runtimeDirectory, "*.dll");
        assembliesToResolve.AddRange(runtimeDirDlls);

        assembliesToResolve.Add(assemblyPath);

        // Create PathAssemblyResolver that can resolve assemblies using the created list.
        var resolver = new PathAssemblyResolver(assembliesToResolve);
        var mlc = new MetadataLoadContext(resolver);
        var assembly = mlc.LoadFromAssemblyPath(assemblyPath);

        return MapIndexesFromAssembly(assembly);
    }

    private CosmosDbIndexMap MapIndexesFromAssembly(Assembly assembly)
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

        foreach (var type in assembly.DefinedTypes)
        {
            try
            {
                var customAttrData = type.GetCustomAttributesData();
                var attr = customAttrData.FirstOrDefault(x => string.Equals(x.AttributeType.Name, nameof(IdConfiguredEntityAttribute)));
                if (attr is object)
                {
                    //We know the container name is the first argument to the attribute
                    var containerName = attr.ConstructorArguments[0];
                    mappedTypesBuilder.Add(new MappedType(type, containerName.Value.ToString()));
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
