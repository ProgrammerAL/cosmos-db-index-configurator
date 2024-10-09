using System;
using System.Runtime.Serialization;

namespace ProgrammerAl.CosmosDbIndexConfigurator.IndexMapper;
[Serializable]
internal class InvalidDbContextTypeException : Exception
{
    public InvalidDbContextTypeException()
    {
    }

    public InvalidDbContextTypeException(string? message) : base(message)
    {
    }

    public InvalidDbContextTypeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
