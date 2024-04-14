using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordLowercaseChecker : PasswordConditionChecker
{
    public PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password)
    {
        var phrase = password.Phrase;
        if (phrase.Any(character => char.IsLetter(character) && char.IsLower(character)))
        {
            return PasswordConditionCheckerResult.Satisfied();
        }
        else
        {
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password muss mindestens einen Kleinbuchstaben enthalten."));
        }
    }
}
