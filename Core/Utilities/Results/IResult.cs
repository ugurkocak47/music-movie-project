namespace Core.Utilities.Results;

public interface IResult 
{ 
    public bool Success { get; }
    public List<string> Messages { get; }
}

