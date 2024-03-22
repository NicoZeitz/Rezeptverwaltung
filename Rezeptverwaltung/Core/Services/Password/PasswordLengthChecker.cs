
using Core.ValueObjects;

namespace Core.Services.Password;

public class PasswordLengthChecker : IPasswordConditionChecker
{
    private readonly int length;

    public PasswordLengthChecker(int length) : base()
    {
        this.length = length;
    }

    public PasswordConditionCheckerResult CheckPassword(ValueObjects.Password password)
    {
        if (password.Phrase.Length < length)
        {
            return PasswordConditionCheckerResult.NotSatisfied(new ErrorMessage($"Password muss mindestens {length} Zeichen lang sein."));
        }
        else
        {
            return PasswordConditionCheckerResult.Satisfied();
        }
    }
}
