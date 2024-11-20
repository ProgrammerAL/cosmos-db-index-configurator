using ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.PropertyMappers;

public class PartitionKeyPropertyMapper
{
    public string? MapPropertyWithAttribute(Type genericType, string indexPath)
    {
        foreach (var property in genericType.GetRuntimeProperties())
        {
            if (property.GetMethod is object
                && !property.GetMethod.IsStatic)
            {
                var includeIndexAttrs = property.GetCustomAttributesData();
                var includeIndexAttr = includeIndexAttrs.FirstOrDefault(x => string.Equals(x.AttributeType.Name, nameof(IncludePartitionKeyAttribute)));
                if (includeIndexAttr is object)
                {
                    return $"/{property.Name}";
                }
            }
        }

        return null;
    }
}
