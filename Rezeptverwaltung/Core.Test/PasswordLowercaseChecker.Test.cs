using Core.Services.Password;
using Core.ValueObjects;
using FluentAssertions;

namespace Core.Test;

public class PasswordLowercaseCheckerTest
{
    [Fact]
    public void TestPasswordWithNoLowercaseIsNotValid()
    {
        var password = new Password("ABCDEFGHIJ");
        var checker = new PasswordLowercaseChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().NotBeNull();
        result.IsSatisfied.Should().BeFalse();
    }

    [Fact]
    public void TestPasswordWithLowercaseIsValid()
    {
        var password = new Password("abcdefghij");
        var checker = new PasswordLowercaseChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }

    [Fact]
    public void TestPasswordWithMixedCaseIsValid()
    {
        var password = new Password("AbCdEfGhIj");
        var checker = new PasswordLowercaseChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }
}