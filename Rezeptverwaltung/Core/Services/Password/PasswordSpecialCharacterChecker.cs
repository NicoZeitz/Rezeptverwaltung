﻿

using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordSpecialCharacterChecker : PasswordConditionChecker
{
    public PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password)
    {
        var phrase = password.Phrase;
        if (phrase.Any(character => !char.IsLetterOrDigit(character)))
        {
            return PasswordConditionCheckerResult.Satisfied();
        }
        else
        {
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage("Password muss mindestens eine Spezialzeichen enthalten."));
        }
    }
}
