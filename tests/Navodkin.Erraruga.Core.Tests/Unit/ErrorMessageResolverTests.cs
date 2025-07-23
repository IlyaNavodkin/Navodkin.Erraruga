using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Dtos.Contracts;
using Navodkin.Erraruga.Core.Services;

namespace Navodkin.Erraruga.Core.Tests.Unit;

public class ErrorMessageResolverTests
{
    [Fact]
    public void WithDefaultRule_ShouldAddRuleToBaseRules()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var rule = new ErrorRule("TEST_001", error => "Default message");

        // Act
        var result = resolver.WithDefaultRule(rule);

        // Assert
        result.Should().Be(resolver);
        resolver.BaseRules.Should().Contain(rule);
    }

    [Fact]
    public void WithRule_ShouldAddRuleToCustomRules()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var rule = new ErrorRule("TEST_002", error => "Custom message");

        // Act
        var result = resolver.WithRule(rule);

        // Assert
        result.Should().Be(resolver);
        resolver.BaseRules.Should().NotContain(rule);
    }

    [Fact]
    public void Resolve_WithNullError_ShouldThrowArgumentNullException()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();

        // Act & Assert
        var action = () => resolver.Resolve(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("error");
    }

    [Fact]
    public void Resolve_WithCustomRule_ShouldUseCustomRule()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("TEST_003", error => "Custom message");
        resolver.WithRule(customRule);
        var error = new AppError("TEST_003", "Original message");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Be("Custom message");
    }

    [Fact]
    public void Resolve_WithCustomRuleAndContext_ShouldUseCustomRule()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("TEST_004", error => "Custom message", "TestContext");
        resolver.WithRule(customRule);
        var error = new AppError("TEST_004", "Original message", "TestContext");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Be("Custom message");
    }

    [Fact]
    public void Resolve_WithCustomRuleReturningEmpty_ShouldUseBaseRule()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("TEST_005", error => "");
        var baseRule = new ErrorRule("TEST_005", error => "Base message");
        resolver.WithRule(customRule).WithDefaultRule(baseRule);
        var error = new AppError("TEST_005", "Original message");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Be("Base message");
    }

    [Fact]
    public void Resolve_WithBaseRulesForceUsed_ShouldSkipCustomRules()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("TEST_006", error => "Custom message");
        var baseRule = new ErrorRule("TEST_006", error => "Base message");
        resolver.WithRule(customRule).WithDefaultRule(baseRule);
        var error = new AppError("TEST_006", "Original message");

        // Act
        var result = resolver.Resolve(error, baseRulesForceUsed: true);

        // Assert
        result.Should().Be("Base message");
    }

    [Fact]
    public void Resolve_WithNoMatchingRules_ShouldUseLowlevelHandle()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var error = new AppError("UNKNOWN_001", "Unknown error", "TestContext");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Contain("Unknown error.");
        result.Should().Contain("UNKNOWN_001: Unknown error");
        result.Should().Contain("Context: TestContext");
    }

    [Fact]
    public void Resolve_WithNoMatchingRulesAndMetadata_ShouldIncludeMetadata()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var error = new AppError("UNKNOWN_002", "Unknown error");
        error.AppendMetadata("key1", "value1");
        error.AppendMetadata("key2", "value2");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Contain("Unknown error.");
        result.Should().Contain("UNKNOWN_002: Unknown error");
        result.Should().Contain("Metadata:");
        result.Should().Contain("key1: value1");
        result.Should().Contain("key2: value2");
    }

    [Fact]
    public void LowlevelHandle_WithNullError_ShouldReturnDefaultMessage()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();

        // Act
        var result = resolver.LowlevelHandle(null!, "TestContext");

        // Assert
        result.Should().Be("Unknown error.\n");
    }

    [Fact]
    public void LowlevelHandle_WithErrorAndContext_ShouldFormatMessage()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var error = new AppError("TEST_007", "Test error", "TestContext");

        // Act
        var result = resolver.LowlevelHandle(error, "TestContext");

        // Assert
        result.Should().Contain("Unknown error.");
        result.Should().Contain("TEST_007: Test error");
        result.Should().Contain("Context: TestContext");
    }

    [Fact]
    public void LowlevelHandle_WithErrorAndMetadata_ShouldIncludeMetadata()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var error = new AppError("TEST_008", "Test error");
        error.AppendMetadata("testKey", "testValue");

        // Act
        var result = resolver.LowlevelHandle(error, null);

        // Assert
        result.Should().Contain("Unknown error.");
        result.Should().Contain("TEST_008: Test error");
        result.Should().Contain("Metadata:");
        result.Should().Contain("testKey: testValue");
    }

    [Fact]
    public void LowlevelHandle_WithNullMetadataValue_ShouldHandleNullGracefully()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var error = new AppError("TEST_009", "Test error");
        error.AppendMetadata("nullKey", null);

        // Act
        var result = resolver.LowlevelHandle(error, null);

        // Assert
        result.Should().Contain("nullKey: ");
    }
}