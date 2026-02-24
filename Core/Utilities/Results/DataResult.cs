namespace Core.Utilities.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    public DataResult(T data, bool success, string message, params object[] parameters) : base(success, message, parameters)
    {
        Data = data;
    }

    public DataResult(T data, bool success, List<string> messages, params object[] parameters) : base(success, messages, parameters)
    {
        Data = data;
    }

    public DataResult(T data, bool success) : base(success)
    {
        Data = data;
    }

    public T Data { get; }
}


