namespace Core.Utilities.Results;

public class SuccessResult : Result
{
    public SuccessResult(string key) : base(true, key)
    {
    }

    public SuccessResult(List<string> keys) : base(true, keys)
    {
    }

    public SuccessResult() : base(true)
    {
    }
}

