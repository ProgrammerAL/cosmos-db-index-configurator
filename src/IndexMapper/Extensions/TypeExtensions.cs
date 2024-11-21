using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper.Extensions;
public static class TypeExtensions
{
    public static bool IsIEnumerableType(this Type targetType)
        => targetType.GetInterfaces().Any(x => x.FullName == "System.Collections.IEnumerable");
    }
