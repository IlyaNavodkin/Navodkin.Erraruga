using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;

namespace Navodkin.Erraruga.Core.Tests.Unit;

public class AppErrorTests
{
    [Fact]
    public void Constructor_WithCodeAndMetadata_ShouldCreateError()
    {
        // Arrange
        var code = "TEST_001";
        var metadata = new Dictionary<string, object> { ["key"] = "value" };

        // Act
        var error = new AppError(code, metadata);

        // Assert
        error.Code.Should().Be(code);
        error.Metadata.Should().BeEquivalentTo(metadata);
        error.Message.Should().BeNull();
        error.Context.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithCodeMessageAndContext_ShouldCreateError()
    {
        // Arrange
        var code = "TEST_002";
        var message = "Test error message";
        var context = "TestContext";

        // Act
        var error = new AppError(code, message, context);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.Context.Should().Be(context);
        error.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithCodeOnly_ShouldCreateError()
    {
        // Arrange
        var code = "TEST_003";

        // Act
        var error = new AppError(code);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().BeNull();
        error.Context.Should().BeNull();
        error.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithCodeAndMessage_ShouldCreateError()
    {
        // Arrange
        var code = "TEST_004";
        var message = "Test error message";

        // Act
        var error = new AppError(code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.Context.Should().BeNull();
        error.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void AppendMetadata_ShouldAddKeyValuePair()
    {
        // Arrange
        var error = new AppError("TEST_005");
        var key = "testKey";
        var value = "testValue";

        // Act
        error.AppendMetadata(key, value);

        // Assert
        error.Metadata.Should().ContainKey(key);
        error.Metadata[key].Should().Be(value);
    }

    [Fact]
    public void AppendMetadata_ShouldOverwriteExistingKey()
    {
        // Arrange
        var error = new AppError("TEST_006");
        var key = "testKey";
        var value1 = "value1";
        var value2 = "value2";

        // Act
        error.AppendMetadata(key, value1);
        error.AppendMetadata(key, value2);

        // Assert
        error.Metadata.Should().ContainKey(key);
        error.Metadata[key].Should().Be(value2);
        error.Metadata.Count.Should().Be(1);
    }

    [Fact]
    public void Metadata_ShouldBeInitializedAsEmptyDictionary()
    {
        // Arrange & Act
        var error = new AppError("TEST_007");

        // Assert
        error.Metadata.Should().NotBeNull();
        error.Metadata.Should().BeEmpty();
    }
}