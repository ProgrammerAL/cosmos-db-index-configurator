namespace ProgrammerAl.CosmosDbIndexConfigurator.ConfigurationLib;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class IdConfiguredEntityAttribute : Attribute
{
    public IdConfiguredEntityAttribute(string containerName)
    { 
        ContainerName = containerName;
    }

    public string ContainerName { get; }
}
