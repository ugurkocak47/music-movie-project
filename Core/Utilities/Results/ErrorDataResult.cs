namespace Core.Utilities.Results;

public class ErrorDataResult<T> : DataResult<T>
{
    public ErrorDataResult(T data, string key) : base(data, false, key)
    {
    }

    public ErrorDataResult(T data, List<string> keys) : base(data, false, keys)
    {
    }

    public ErrorDataResult(T data) : base(data, false)
    {
    }

    public ErrorDataResult(string key) : base(default, false, key)
    {
    }

    public ErrorDataResult(List<string> keys) : base(default, false, keys)
    {
    }

    public ErrorDataResult() : base(default, false)
    {
    }
}


