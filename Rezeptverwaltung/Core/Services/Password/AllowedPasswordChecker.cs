using Core.ValueObjects;

namespace Core.Services.Password;

public class AllowedPasswordChecker
{
    private readonly IEnumerable<PasswordConditionChecker> passwordConditionCheckers;

    public AllowedPasswordChecker(IEnumerable<PasswordConditionChecker> passwordConditionCheckers)
        : base()
    {
        this.passwordConditionCheckers = passwordConditionCheckers;
    }

    public IEnumerable<ErrorMessage> CheckPassword(ValueObjects.Password password)
    {
        return from passwordConditionChecker in passwordConditionCheckers
               let conditionResult = passwordConditionChecker.CheckPassword(password)
               where conditionResult.IsNotSatisfied
               select conditionResult.Message!.Value;
    }
}
