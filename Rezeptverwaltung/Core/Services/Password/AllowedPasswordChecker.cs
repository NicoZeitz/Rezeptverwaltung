﻿using Core.ValueObjects;

namespace Core.Services.Password;

public class AllowedPasswordChecker
{
    private readonly IEnumerable<IPasswordConditionChecker> passwordConditionCheckers;

    public AllowedPasswordChecker() : base()
    {
        passwordConditionCheckers = new IPasswordConditionChecker[]
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