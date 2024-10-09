namespace ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class IncludePartitionKeyAttribute : Attribute
{
}
