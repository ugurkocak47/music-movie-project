using System.Reflection;
using Microsoft.Extensions.Localization;

namespace Core.Utilities.Localization;

public class ApplicationResource { }

public class LocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(IStringLocalizerFactory factory)
    {
        var type = typeof(ApplicationResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
        _localizer = factory.Create(nameof(ApplicationResource), assemblyName.Name!);
    }

    public string Translate(string key, params object[] parameters)
    {
        var translation = _localizer[key];
        return string.Format(translation ?? key, parameters);
    }

    public async Task<string> TranslateAsync(Task<string> keyTask, params Task<object>[] parameterTasks)
    {
        var key = await keyTask;
        var parameters = await Task.WhenAll(parameterTasks);

        var translation = _localizer[key];
        return string.Format(translation ?? key, parameters);
    }
}
