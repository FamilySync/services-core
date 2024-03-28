namespace FamilySync.Core.Example.Services;

public interface IExampleService
{
    Task<string> GetExampleData();
}

public class ExampleService : IExampleService
{
    public async Task<string> GetExampleData()
    {
        await Task.Delay(250);

        return "This is example data!";
    }
}