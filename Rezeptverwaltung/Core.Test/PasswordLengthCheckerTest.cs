using Core.Services.Password;
using Core.ValueObjects;
using FluentAssertions;

namespace Core.Test;

public class PasswordLengthCheckerTest
{
    [Fact]
    public void TestPasswordWithLengthLessThanRequiredIsNotValid()
    {
        var password = new Password("1234567890");
        var passwordLength = password.Phrase.Length;
        var checker = new PasswordLengthChecker(passwordLength + 1);

        var result = checker.CheckPassword(password);

        result.Message.Should().NotBeNull();
        result.IsSatisfied.Should().BeFalse();
    }

    [Fact]
    public void TestPasswordWithSameLengthIsValid()
    {
        var password = new Password("1234567890");
        var passwordLength = password.Phrase.Length;
        var checker = new PasswordLengthChecker(passwordLength);

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }

    [Fact]
    public void TestPasswordWithLengthGreaterThanRequiredIsValid()
    {
        var password = new Password("1234567890");
        var passwordLength = password.Phrase.Length;
        var checker = new PasswordLengthChecker(passwordLength - 1);

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }
}