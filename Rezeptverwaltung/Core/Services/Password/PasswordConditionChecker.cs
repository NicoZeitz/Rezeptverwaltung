using Core.ValueObjects;

namespace Core.Services.Password;

public sealed class PasswordConditionCheckerResult
{
    public readonly bool IsSatisfied;
    public readonly ErrorMessage? Message;

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

public interface PasswordConditionChecker
{
    PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password);
}
