using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;
public static class Utilities
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

}
