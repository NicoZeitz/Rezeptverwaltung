using Core.Services.Password;
using Core.ValueObjects;
using FluentAssertions;

namespace Core.Test;

public class PasswordSpecialCharacterCheckerTest
{
    [Fact]
    public void TestPasswordWithNoSpecialCharactersIsNotValid()
    {
        var password = new Password("123abcABC");
        var checker = new PasswordSpecialCharacterChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().NotBeNull();
        result.IsSatisfied.Should().BeFalse();
    }

    [Fact]
    public void TestPasswordWithSpecialCharactersIsValid()
    {
        var password = new Password("!123abcABC");
        var checker = new PasswordSpecialCharacterChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }
}