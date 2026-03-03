namespace TaskManager.Application.Common.Models;

public class Reslt<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    


    private Reslt() { }

    public static Reslt<T> Success(T value)
    {
        return new Reslt<T>
        {
            IsSuccess = true,
            Value = value
        };
    }

    public static Reslt<T> Failure(string error)
    {
        return new Reslt<T>
        {
            IsSuccess = false,
            Error = error
        };
    }

}
