using Core.Services.Password;
using Core.ValueObjects;
using FluentAssertions;

namespace Core.Test;

public class PasswordLengthCheckerTest
{
    [Fact]
    public void TestPasswordWithLengthLessThanRequiredIsNotValid()
    {
        var password = new Password("randomPassword");
        var minimumPasswordLength = password.Phrase.Length + 1;

        var checker = new PasswordLengthChecker(minimumPasswordLength);

        var result = checker.CheckPassword(password);

        result.Message.Should().NotBeNull();
        result.IsSatisfied.Should().BeFalse();
    }

    [Fact]
    public void TestPasswordWithSameLengthIsValid()
    {
        var password = new Password("randomPassword");
        var minimumPasswordLength = password.Phrase.Length;

        var checker = new PasswordLengthChecker(minimumPasswordLength);

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }

    [Fact]
    public void TestPasswordWithLengthGreaterThanRequiredIsValid()
    {
        var password = new Password("randomPassword");
        var minimumPasswordLength = password.Phrase.Length - 1;

        var checker = new PasswordLengthChecker(minimumPasswordLength);

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }
}