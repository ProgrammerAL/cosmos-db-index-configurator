using System;
using System.Collections.Generic;
using System.Text;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.Extensions;
public static class TypeExtensions
{
    public static bool IsAssignableTo(this Type targetType, Type? other) => other?.IsAssignableFrom(targetType) ?? false;
}
