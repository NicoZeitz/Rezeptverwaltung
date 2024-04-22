using Core.ValueObjects;

namespace Core.Services.Password;

public class AllowedPasswordChecker
{
    private readonly IList<PasswordConditionChecker> passwordConditionCheckers;

    public AllowedPasswordChecker() : this(Enumerable.Empty<PasswordConditionChecker>()) { }

    public AllowedPasswordChecker(params PasswordConditionChecker[] passwordConditionCheckers)
        : this(passwordConditionCheckers as IEnumerable<PasswordConditionChecker>) { }

    public AllowedPasswordChecker(IEnumerable<PasswordConditionChecker> passwordConditionCheckers)
        : base()
    {
        this.passwordConditionCheckers = passwordConditionCheckers.ToList();
    }

    public void AddPasswordConditionChecker(PasswordConditionChecker passwordConditionChecker)
    {
        passwordConditionCheckers.Add(passwordConditionChecker);
    }

    public void RemovePasswordConditionChecker(PasswordConditionChecker passwordConditionChecker)
    {
        passwordConditionCheckers.Remove(passwordConditionChecker);
    }

    public IEnumerable<ErrorMessage> CheckPassword(ValueObjects.Password password)
    {
        return from passwordConditionChecker in passwordConditionCheckers
               let conditionResult = passwordConditionChecker.CheckPassword(password)
               where conditionResult.IsNotSatisfied
               select conditionResult.Message!.Value;
    }
}
