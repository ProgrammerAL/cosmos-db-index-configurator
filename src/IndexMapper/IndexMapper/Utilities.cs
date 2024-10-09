using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.Extensions;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;
public static class Utilities
{
    public static bool IsPropertyScalar(PropertyInfo property)
    {
        return property.PropertyType.IsAssignableTo(typeof(string))
               || property.PropertyType.IsAssignableTo(typeof(int))
               || property.PropertyType.IsAssignableTo(typeof(decimal))
               || property.PropertyType.IsAssignableTo(typeof(double))
               || property.PropertyType.IsAssignableTo(typeof(float))
               || property.PropertyType.IsAssignableTo(typeof(long))
               || property.PropertyType.IsAssignableTo(typeof(short))
               || property.PropertyType.IsAssignableTo(typeof(bool))
               || property.PropertyType.IsAssignableTo(typeof(Guid))
               || property.PropertyType.IsAssignableTo(typeof(DateTime))
               || property.PropertyType.Name == "DateTimeOffset"
               || property.PropertyType.Name == "DateOnly"
               || property.PropertyType.Name == "TimeOnly";
    }

}
