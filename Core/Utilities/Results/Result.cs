using Microsoft.AspNetCore.Identity;
using Core.Utilities.Localization;

namespace Core.Utilities.Results;

public class Result : IResult
{
    public Result(bool success, string key, params object[] parameters)
    {
        Success = success;
        Messages = new List<string> { key };
    }

    public Result(bool success, List<string> keys, params object[] parameters)
    {
        Success = success;
        Messages = keys ?? new List<string>();
    }

    public Result(bool success)
    {
        Success = success;
        Messages = new List<string>();
    }

    public bool Success { get; }
    public List<string> Messages { get; }
}

