using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Dtos.Contracts;

namespace Navodkin.Erraruga.Core.Tests.Unit;

public class ErrorRuleTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateRule()
    {
        // Arrange
        var code = "TEST_001";
        var context = "TestContext";
        Func<IAppError, string> handler = error => "Test message";

        // Act
        var rule = new ErrorRule(code, handler, context);

        // Assert
        rule.Code.Should().Be(code);
        rule.Context.Should().Be(context);
        rule.Handler.Should().Be(handler);
    }

    [Fact]
    public void Constructor_WithoutContext_ShouldCreateRuleWithNullContext()
    {
        // Arrange
        var code = "TEST_002";
        Func<IAppError, string> handler = error => "Test message";

        // Act
        var rule = new ErrorRule(code, handler);

        // Assert
        rule.Code.Should().Be(code);
        rule.Context.Should().BeNull();
        rule.Handler.Should().Be(handler);
    }

    [Fact]
    public void Constructor_WithNullCode_ShouldThrowArgumentNullException()
    {
        // Arrange
        string code = null!;
        Func<IAppError, string> handler = error => "Test message";

        // Act & Assert
        var action = () => new ErrorRule(code, handler);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("code");
    }

    [Fact]
    public void Constructor_WithNullHandler_ShouldThrowArgumentNullException()
    {
        // Arrange
        var code = "TEST_003";
        Func<IAppError, string> handler = null!;

        // Act & Assert
        var action = () => new ErrorRule(code, handler);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("handler");
    }

    [Fact]
    public void Key_ShouldReturnCodeAndContextTuple()
    {
        // Arrange
        var code = "TEST_004";
        var context = "TestContext";
        Func<IAppError, string> handler = error => "Test message";
        var rule = new ErrorRule(code, handler, context);

        // Act
        var key = rule.Key;

        // Assert
        key.Should().Be((code, context));
    }

    [Fact]
    public void Key_WithNullContext_ShouldReturnCodeAndNullContext()
    {
        // Arrange
        var code = "TEST_005";
        Func<IAppError, string> handler = error => "Test message";
        var rule = new ErrorRule(code, handler);

        // Act
        var key = rule.Key;

        // Assert
        key.Should().Be((code, null));
    }

    [Fact]
    public void Handler_ShouldProcessErrorCorrectly()
    {
        // Arrange
        var code = "TEST_006";
        var expectedMessage = "Processed error message";
        Func<IAppError, string> handler = error => expectedMessage;
        var rule = new ErrorRule(code, handler);
        var error = new AppError(code, "Original message");

        // Act
        var result = rule.Handler(error);

        // Assert
        result.Should().Be(expectedMessage);
    }
}