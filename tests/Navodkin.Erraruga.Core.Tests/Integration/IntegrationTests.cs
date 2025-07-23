using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Services;
using Navodkin.Erraruga.Core.Exceptions;
using Navodkin.Erraruga.Core.Extensions;
using Navodkin.Erraruga.Core.Dtos.Contracts;

namespace Navodkin.Erraruga.Core.Tests.Integration;

public class IntegrationTests
{
    [Fact]
    public void CompleteWorkflow_WithCustomAndDefaultRules_ShouldWorkCorrectly()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("VALIDATION_ERROR", error => "Custom validation error", "UserInput");
        var defaultRule = new ErrorRule("VALIDATION_ERROR", error => "Default validation error");

        resolver.WithRule(customRule).WithDefaultRule(defaultRule);

        var error = new AppError("VALIDATION_ERROR", "Field is required", "UserInput");
        error.AppendMetadata("field", "email");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Be("Custom validation error");
    }

    [Fact]
    public void CompleteWorkflow_WithFallbackToDefaultRule_ShouldWorkCorrectly()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("VALIDATION_ERROR", error => "", "UserInput");
        var defaultRule = new ErrorRule("VALIDATION_ERROR", error => "Default validation error");

        resolver.WithRule(customRule).WithDefaultRule(defaultRule);

        var error = new AppError("VALIDATION_ERROR", "Field is required", "UserInput");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Be("Default validation error");
    }

    [Fact]
    public void CompleteWorkflow_WithBuilder_ShouldWorkCorrectly()
    {
        // Arrange
        var builder = new ErrorMessageResolverBuilder();
        var rule = new ErrorRule("SYSTEM_ERROR", error => "System error occurred");

        var resolver = builder.WithDefaultRule(rule).Build();
        var error = new AppError("SYSTEM_ERROR", "Database connection failed");

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Be("System error occurred");
    }

    [Fact]
    public void CompleteWorkflow_WithExceptionThrowing_ShouldWorkCorrectly()
    {
        // Arrange
        var error = new AppError("CRITICAL_ERROR", "Critical system failure");
        error.AppendMetadata("component", "Database");

        // Act & Assert
        var action = () => error.Throw();
        var exception = action.Should().Throw<AppException>().Subject.First();

        exception.Errors.Should().ContainSingle();
        exception.Errors.First().Code.Should().Be("CRITICAL_ERROR");
        exception.Errors.First().Message.Should().Be("Critical system failure");
        exception.Errors.First().Metadata["component"].Should().Be("Database");
    }

    [Fact]
    public void CompleteWorkflow_WithMultipleErrorsAndException_ShouldWorkCorrectly()
    {
        // Arrange
        var errors = new List<IAppError>
        {
            new AppError("VALIDATION_ERROR", "Email is invalid"),
            new AppError("AUTH_ERROR", "User not authenticated")
        };

        // Act & Assert
        var action = () => errors.Throw();
        var exception = action.Should().Throw<AppException>().Subject.First();

        exception.Errors.Should().HaveCount(2);
        exception.Message.Should().Contain("(VALIDATION_ERROR): Email is invalid");
        exception.Message.Should().Contain("(AUTH_ERROR): User not authenticated");
    }

    [Fact]
    public void CompleteWorkflow_WithUnknownError_ShouldUseLowlevelHandle()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var error = new AppError("UNKNOWN_ERROR", "Unknown error occurred", "TestContext");
        error.AppendMetadata("timestamp", DateTime.Now.ToString());

        // Act
        var result = resolver.Resolve(error);

        // Assert
        result.Should().Contain("Unknown error.");
        result.Should().Contain("UNKNOWN_ERROR: Unknown error occurred");
        result.Should().Contain("Context: TestContext");
        result.Should().Contain("Metadata:");
        result.Should().Contain("timestamp:");
    }

    [Fact]
    public void CompleteWorkflow_WithForceBaseRules_ShouldSkipCustomRules()
    {
        // Arrange
        var resolver = new ErrorMessageResolver();
        var customRule = new ErrorRule("TEST_ERROR", error => "Custom message", "Context1");
        var baseRule = new ErrorRule("TEST_ERROR", error => "Base message");

        resolver.WithRule(customRule).WithDefaultRule(baseRule);

        var error = new AppError("TEST_ERROR", "Original message", "Context1");

        // Act
        var result = resolver.Resolve(error, baseRulesForceUsed: true);

        // Assert
        result.Should().Be("Base message");
    }
}