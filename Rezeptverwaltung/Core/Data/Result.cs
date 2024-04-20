using Core.ValueObjects;

namespace Core.Data;

public sealed class Result<T> : IEquatable<Result<T>>
{
    public readonly bool IsSuccessful;
    public readonly T? Value;
    public readonly IEnumerable<ErrorMessage> ErrorMessages;

    public bool IsError => !IsSuccessful;

    private Result(
        bool isSuccessful,
        T? value,
        IEnumerable<ErrorMessage> errorMessages)
        : base()
    {
        IsSuccessful = isSuccessful;
        Value = value;
        ErrorMessages = errorMessages;
    }

    public static Result<T> Successful(T value)
    {
        return new Result<T>(true, value, Enumerable.Empty<ErrorMessage>());
    }

    public static Result<T> Error(ErrorMessage errorMessage)
    {
        return new Result<T>(false, default, new[] { errorMessage });
    }

    public static Result<T> Errors(IEnumerable<ErrorMessage> errorMessages)
    {
        return new Result<T>(false, default, errorMessages);
    }

    public bool Equals(Result<T>? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        if (IsSuccessful)
            return other.IsSuccessful && EqualityComparer<T>.Default.Equals(Value, other.Value);

        return ErrorMessages.SequenceEqual(other.ErrorMessages);
    }

    public override int GetHashCode() => IsSuccessful ? Value?.GetHashCode() ?? 0 : ErrorMessages.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as Result<T>);

    public static bool operator ==(Result<T>? left, Result<T>? right) =>
        ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(Result<T>? left, Result<T>? right) => !(left == right);
}