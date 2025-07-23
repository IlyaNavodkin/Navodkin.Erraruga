using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Services;

namespace Navodkin.Erraruga.Core.Tests.Unit;

public class ErrorMessageResolverBuilderTests
{
    [Fact]
    public void WithDefaultRule_ShouldAddRuleToInstance()
    {
        // Arrange
        var builder = new ErrorMessageResolverBuilder();
        var rule = new ErrorRule("TEST_001", error => "Default message");

        // Act
        var result = builder.WithDefaultRule(rule);

        // Assert
        result.Should().Be(builder);
        builder.Instance.BaseRules.Should().Contain(rule);
    }

    [Fact]
    public void Build_ShouldReturnInstance()
    {
        // Arrange
        var builder = new ErrorMessageResolverBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().Be(builder.Instance);
        result.Should().BeOfType<ErrorMessageResolver>();
    }

    [Fact]
    public void Instance_ShouldBeInitialized()
    {
        // Arrange & Act
        var builder = new ErrorMessageResolverBuilder();

        // Assert
        builder.Instance.Should().NotBeNull();
        builder.Instance.Should().BeOfType<ErrorMessageResolver>();
    }

    [Fact]
    public void WithDefaultRule_ShouldBeChainable()
    {
        // Arrange
        var builder = new ErrorMessageResolverBuilder();
        var rule1 = new ErrorRule("TEST_002", error => "Message 1");
        var rule2 = new ErrorRule("TEST_003", error => "Message 2");

        // Act
        var result = builder
            .WithDefaultRule(rule1)
            .WithDefaultRule(rule2);

        // Assert
        result.Should().Be(builder);
        builder.Instance.BaseRules.Should().Contain(rule1);
        builder.Instance.BaseRules.Should().Contain(rule2);
        builder.Instance.BaseRules.Count.Should().Be(2);
    }
}