using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;

/// <summary>
/// Checks type info.
/// We have to check based on known type names because the assemblies we're checking are loaded using reflection only
/// This means if a type is loaded, for example String, that's not the same String type that's running in this code. It's in a different context.
/// </summary>
public static class TypeCheckUtilities
{
    public static bool IsPropertyScalar(PropertyInfo property)
    {
        return
                  property.PropertyType.FullName == "System.String"
               || property.PropertyType.FullName == "System.Int32"
               || property.PropertyType.FullName == "System.Decimal"
               || property.PropertyType.FullName == "System.Double"
               || property.PropertyType.FullName == "System.Single"
               || property.PropertyType.FullName == "System.Int64"
               || property.PropertyType.FullName == "System.Int16"
               || property.PropertyType.FullName == "System.Boolean"
               || property.PropertyType.FullName == "System.Guid"
               || property.PropertyType.FullName == "System.DateTime"
               || property.PropertyType.Name == "DateTimeOffset"
               || property.PropertyType.Name == "DateOnly"
               || property.PropertyType.Name == "TimeOnly";
    }

    public static bool IsPropertyIgnored(PropertyInfo property)
    {
        return property.PropertyType.FullName == "System.Type";
    }
}
