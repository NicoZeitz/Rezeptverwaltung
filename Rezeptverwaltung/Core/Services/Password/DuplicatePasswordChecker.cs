namespace Core.Services.Password;

public class DuplicatePasswordChecker
{
    public bool IsSamePassword(ValueObjects.Password password, ValueObjects.Password passwordRepeat)
    {
        return password.Phrase == passwordRepeat.Phrase;
    }
}