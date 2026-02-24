namespace Core.Utilities.Results;

public class SuccessDataResult<T> : DataResult<T>
{
    public SuccessDataResult(T data, string key) : base(data, true, key)
    {
    }

    public SuccessDataResult(T data, List<string> keys) : base(data, true, keys)
    {
    }

    public SuccessDataResult(T data) : base(data, true)
    {
    }

    public SuccessDataResult(string key) : base(default, true, key)
    {
    }

    public SuccessDataResult(List<string> keys) : base(default, true, keys)
    {
    }

    public SuccessDataResult() : base(default, true)
    {
    }
}

