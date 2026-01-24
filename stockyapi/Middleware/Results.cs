namespace stockyapi.Middleware;

public sealed class Result<T>
{
    public readonly bool IsSuccess;
    public readonly T Value;
    public readonly Failure Failure;
    public bool IsFailure => !IsSuccess;
    
    private Result(bool success, T value, Failure failure)
        => (IsSuccess, Value, Failure) = (success, value, failure);

    public static Result<T> Success(T value) => new(true, value, new None());
    public static Result<T> Fail(Failure failure) => new(false, default!, failure);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Failure failure) => Fail(failure);
}