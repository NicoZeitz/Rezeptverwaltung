using Core.ValueObjects;

namespace Core.Services.Password;

internal class PasswordDigitChecker : IPasswordConditionChecker
{
    public PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password)
    {
        var phrase = password.Phrase;
        if (phrase.Any(char.IsDigit))
        {
            return PasswordConditionCheckerResult.Satisfied();
        }
        else
        {
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password must contain at least one number."));
        }
    }
}
