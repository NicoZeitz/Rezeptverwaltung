using Core.Services.Password;
using Core.ValueObjects;
using FluentAssertions;

namespace Core.Test;

public class PasswordDigitCheckerTest
{
    [Fact]
    public void TestPasswordWithNoDigitsIsNotValid()
    {
        var password = new Password("abcdefghij");
        var checker = new PasswordDigitChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().NotBeNull();
        result.IsSatisfied.Should().BeFalse();
    }

    [Fact]
    public void TestPasswordWithDigitsIsValid()
    {
        var password = new Password("1 abcdefghij");
        var checker = new PasswordDigitChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }

    [Fact]
    public void TestPasswordWithDigitsAndSpecialCharactersIsValid()
    {
        var password = new Password("1 abcdefghij!@#$%^&*()");
        var checker = new PasswordDigitChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }

    [Fact]
    public void TestOnlyDigitsIsValid()
    {
        var password = new Password("123");
        var checker = new PasswordDigitChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }
}