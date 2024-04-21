using Core.Services.Password;
using Core.ValueObjects;
using FluentAssertions;

namespace Core.Test;

public class PasswordUppercaseCheckerTest
{
    [Fact]
    public void TestPasswordWithNoUppercaseIsNotValid()
    {
        var password = new Password("abcdefghij");
        var checker = new PasswordUppercaseChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().NotBeNull();
        result.IsSatisfied.Should().BeFalse();
    }

    [Fact]
    public void TestPasswordWithUppercaseIsValid()
    {
        var password = new Password("ABCDEFGHIJ");
        var checker = new PasswordUppercaseChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }

    [Fact]
    public void TestPasswordWithMixedCaseIsValid()
    {
        var password = new Password("AbCdEfGhIj");
        var checker = new PasswordUppercaseChecker();

        var result = checker.CheckPassword(password);

        result.Message.Should().BeNull();
        result.IsSatisfied.Should().BeTrue();
    }
}