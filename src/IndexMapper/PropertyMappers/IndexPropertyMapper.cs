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
    private const int MaxDepthLimit = 50;

    public ImmutableArray<string> MapPropertiesWithAttribute(Type genericType, string indexPath)
    {
        if (indexPath.Count(x => x == '/') > MaxDepthLimit)
        {
            return ImmutableArray<string>.Empty;
        }

        var builder = ImmutableArray.CreateBuilder<string>();

        foreach (var property in genericType.GetRuntimeProperties())
        {
            if (TypeCheckUtilities.IsPropertyIgnored(property))
            {
                continue;
            }
            else if (property.GetMethod is object
                && !property.GetMethod.IsStatic)
            {
                var propertyIndexPath = $"{indexPath}{property.Name}/";

                var includeIndexAttrs = property.GetCustomAttributesData();
                var includeIndexAttr = includeIndexAttrs.FirstOrDefault(x => string.Equals(x.AttributeType.Name, nameof(IncludeIndexAttribute)));
                if (includeIndexAttr is object)
                {
                    if (TypeCheckUtilities.IsPropertyScalar(property))
                    {
                        builder.Add($"{propertyIndexPath}?");
                    }
                    else
                    {
                        builder.Add($"{propertyIndexPath}*");
                    }
                }
                else if (property.PropertyType.IsIEnumerableType())
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
                    && !TypeCheckUtilities.IsPropertyScalar(property))
                {
                    var objectSubTypes = MapPropertiesWithAttribute(property.PropertyType, $"{propertyIndexPath}");
                    builder.AddRange(objectSubTypes);
                }
            }
        }

        return builder.ToImmutable();
    }
}
