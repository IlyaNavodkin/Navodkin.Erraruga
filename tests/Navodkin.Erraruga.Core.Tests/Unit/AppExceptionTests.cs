using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Dtos.Contracts;
using Navodkin.Erraruga.Core.Exceptions;

namespace Navodkin.Erraruga.Core.Tests.Unit;

public class AppExceptionTests
{
    [Fact]
    public void Constructor_WithSingleError_ShouldCreateException()
    {
        // Arrange
        var error = new AppError("TEST_001", "Test error message");

        // Act
        var exception = new AppException(error);

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Errors.Should().Contain(error);
        exception.Message.Should().Contain("App errors:");
        exception.Message.Should().Contain("(TEST_001): Test error message");
    }

    [Fact]
    public void Constructor_WithMultipleErrors_ShouldCreateException()
    {
        // Arrange
        var errors = new List<IAppError>
        {
            new AppError("TEST_002", "Error 1"),
            new AppError("TEST_003", "Error 2")
        };

        // Act
        var exception = new AppException(errors);

        // Assert
        exception.Errors.Should().HaveCount(2);
        exception.Errors.Should().BeEquivalentTo(errors);
        exception.Message.Should().Contain("App errors:");
        exception.Message.Should().Contain("(TEST_002): Error 1");
        exception.Message.Should().Contain("(TEST_003): Error 2");
    }

    [Fact]
    public void Constructor_WithEmptyErrorsCollection_ShouldCreateException()
    {
        // Arrange
        var errors = new List<IAppError>();

        // Act
        var exception = new AppException(errors);

        // Assert
        exception.Errors.Should().BeEmpty();
        exception.Message.Should().Be("No error details provided.");
    }

    [Fact]
    public void Constructor_WithNullErrorsCollection_ShouldCreateException()
    {
        // Arrange
        ICollection<IAppError> errors = null!;

        // Act
        var exception = new AppException(errors);

        // Assert
        exception.Errors.Should().BeNull();
        exception.Message.Should().Be("No error details provided.");
    }

    [Fact]
    public void Message_WithSingleError_ShouldFormatCorrectly()
    {
        // Arrange
        var error = new AppError("TEST_004", "Single error", "TestContext");

        // Act
        var exception = new AppException(error);

        // Assert
        exception.Message.Should().Be("App errors: (TEST_004): Single error");
    }

    [Fact]
    public void Message_WithMultipleErrors_ShouldFormatCorrectly()
    {
        // Arrange
        var errors = new List<IAppError>
        {
            new AppError("TEST_005", "First error"),
            new AppError("TEST_006", "Second error")
        };

        // Act
        var exception = new AppException(errors);

        // Assert
        exception.Message.Should().Be("App errors: (TEST_005): First error; (TEST_006): Second error");
    }

    [Fact]
    public void Errors_ShouldBeReadOnly()
    {
        // Arrange
        var error = new AppError("TEST_007", "Test error");
        var exception = new AppException(error);

        // Act & Assert
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().ContainSingle();
    }
}