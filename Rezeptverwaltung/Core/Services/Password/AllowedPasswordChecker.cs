using Core.ValueObjects;

namespace Core.Services.Password;

public class AllowedPasswordChecker
{
    private readonly IEnumerable<PasswordConditionChecker> passwordConditionCheckers;

    public AllowedPasswordChecker() : base()
    {
        // TODO: move to main
        passwordConditionCheckers = new PasswordConditionChecker[]
        {
            new PasswordLengthChecker(8),
            new PasswordUppercaseChecker(),
            new PasswordLowercaseChecker(),
            new PasswordDigitChecker(),
            new PasswordSpecialCharacterChecker()
        };
    }

    public IEnumerable<ErrorMessage> CheckPassword(ValueObjects.Password password)
    {
        return from passwordConditionChecker in passwordConditionCheckers
               let conditionResult = passwordConditionChecker.CheckPassword(password)
               where conditionResult.IsNotSatisfied
               select conditionResult.Message!.Value;
    }
}
