using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordDigitChecker : PasswordConditionChecker
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
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password muss mindestens eine Zahl enthalten."));
        }
    }
}
