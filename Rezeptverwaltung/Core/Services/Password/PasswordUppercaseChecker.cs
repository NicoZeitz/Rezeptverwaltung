
using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordUppercaseChecker : IPasswordConditionChecker
{
    public PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password)
    {
        var phrase = password.Phrase;
        if (phrase.Any(character => char.IsLetter(character) && char.IsUpper(character)))
        {
            return PasswordConditionCheckerResult.Satisfied();
        }
        else
        {
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password must contain at least one uppercase letter."));
        }
    }
}
