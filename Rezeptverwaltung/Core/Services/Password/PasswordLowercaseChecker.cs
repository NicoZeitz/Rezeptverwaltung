using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordLowercaseChecker : IPasswordConditionChecker
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
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password must contain at least one lowercase letter."));
        }
    }
}
