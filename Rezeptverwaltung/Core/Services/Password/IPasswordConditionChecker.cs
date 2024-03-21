using Core.ValueObjects;

namespace Core.Services.Password;

public sealed class PasswordConditionCheckerResult
{
    public bool IsSatisfied { get; }
    public ErrorMessage? Message { get; }

    public bool IsNotSatisfied => !IsSatisfied;

    private PasswordConditionCheckerResult(bool isSatisfied, ErrorMessage? message)
    {
        IsSatisfied = isSatisfied;
        Message = message;
    }

    public static PasswordConditionCheckerResult Satisfied()
    {
        return new PasswordConditionCheckerResult(true, null);
    }

    public static PasswordConditionCheckerResult NotSatisfied(ErrorMessage message)
    {
        return new PasswordConditionCheckerResult(false, message);
    }
}

public interface IPasswordConditionChecker
{
    PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password);
}
