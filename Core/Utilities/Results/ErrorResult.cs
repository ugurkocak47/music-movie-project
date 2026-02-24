using Microsoft.AspNetCore.Identity;

namespace Core.Utilities.Results;

public class ErrorResult : Result
{
    public ErrorResult(string key) : base(false, key)
    {
    }

    public ErrorResult(List<string> keys) : base(false, keys)
    {
    }

    public ErrorResult() : base(false)
    {
    }
}


