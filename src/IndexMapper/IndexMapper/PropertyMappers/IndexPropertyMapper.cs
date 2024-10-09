using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;
using ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.Extensions;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.PropertyMappers;

public class IndexPropertyMapper
{
    public ImmutableArray<string> MapPropertiesWithAttribute(Type genericType, string indexPath)
    {
        var builder = ImmutableArray.CreateBuilder<string>();

        foreach (var property in genericType.GetRuntimeProperties())
        {
            if (property.GetMethod is object
                && !property.GetMethod.IsStatic)
            {
                var propertyIndexPath = $"{indexPath}{property.Name}/";

                var includeIndexAttr = property.GetCustomAttribute<IncludeIndexAttribute>();
                if (includeIndexAttr is object)
                {
                    if (Utilities.IsPropertyScalar(property))
                    {
                        builder.Add($"{propertyIndexPath}?");
                    }
                    else
                    {
                        builder.Add($"{propertyIndexPath}*");
                    }
                }
                else if (property.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                {
                    if (property.PropertyType.IsGenericType)
                    {
                        //List<> or Something like that
                        foreach (var genericArgumentType in property.PropertyType.GenericTypeArguments)
                        {
                            var collectionSubTypes = MapPropertiesWithAttribute(genericArgumentType, $"{propertyIndexPath}[]/");
                            builder.AddRange(collectionSubTypes);
                        }
                    }
                    else if (property.PropertyType.IsArray)
                    {
                        //An array
                        var elementType = property.PropertyType.GetElementType();
                        if (elementType is object)
                        {
                            var collectionSubTypes = MapPropertiesWithAttribute(elementType, $"{propertyIndexPath}[]/");
                            builder.AddRange(collectionSubTypes);
                        }
                    }
                }
                else if (property.PropertyType != typeof(object)
                    && property.PropertyType != typeof(Type)
                    && !Utilities.IsPropertyScalar(property))
                {
                    var objectSubTypes = MapPropertiesWithAttribute(property.PropertyType, $"{propertyIndexPath}");
                    builder.AddRange(objectSubTypes);
                }
            }
        }

        return builder.ToImmutable();
    }
}
