
using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordUppercaseChecker : PasswordConditionChecker
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
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password muss mindestens einen Großbuchstaben enthalten."));
        }
    }
}
