namespace FamilySync.Core.Helpers.Settings;

public class ConfigurationSettings
{
    public AuthenticationSettings Authentication { get; set; } = new();

    public ServiceSettings Service { get; set; } = new();
    public IncludeSettings Include { get; set; } = new();
}