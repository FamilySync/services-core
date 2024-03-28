namespace FamilySync.Core.Helpers.Settings;

public class ConfigurationSettings
{
    public ServiceSettings Service { get; set; } = new();
    public IncludeSettings Include { get; set; } = new();
}