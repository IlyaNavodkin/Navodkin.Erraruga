using FluentAssertions;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Dtos.Contracts;
using Navodkin.Erraruga.Core.Exceptions;
using Navodkin.Erraruga.Core.Extensions;

namespace Navodkin.Erraruga.Core.Tests.Unit;

public class AppErrorExtensionsTests
{
    [Fact]
    public void Throw_WithSingleError_ShouldThrowAppException()
    {
        // Arrange
        var error = new AppError("TEST_001", "Test error message");

        // Act & Assert
        var action = () => error.Throw();

        action.Should().Throw<AppException>()
            .WithMessage("*App errors: (TEST_001): Test error message*");
    }

    [Fact]
    public void Throw_WithSingleError_ShouldThrowExceptionWithCorrectError()
    {
        // Arrange
        var error = new AppError("TEST_002", "Test error");

        // Act & Assert
        var exception = Assert.Throws<AppException>(() => error.Throw());
        exception.Errors.Should().ContainSingle();
        exception.Errors.Should().Contain(error);
    }

    [Fact]
    public void Throw_WithListOfErrors_ShouldThrowAppException()
    {
        // Arrange
        var errors = new List<IAppError>
        {
            new AppError("TEST_003", "Error 1"),
            new AppError("TEST_004", "Error 2")
        };

        // Act & Assert
        var action = () => errors.Throw();
        action.Should().Throw<AppException>()
            .WithMessage("*App errors: (TEST_003): Error 1; (TEST_004): Error 2*");
    }

    [Fact]
    public void Throw_WithListOfErrors_ShouldThrowExceptionWithCorrectErrors()
    {
        // Arrange
        var errors = new List<IAppError>
        {
            new AppError("TEST_005", "Error 1"),
            new AppError("TEST_006", "Error 2")
        };

        // Act & Assert
        var exception = Assert.Throws<AppException>(() => errors.Throw());
        exception.Errors.Should().HaveCount(2);
        exception.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Throw_WithEmptyList_ShouldThrowAppException()
    {
        // Arrange
        var errors = new List<IAppError>();

        // Act & Assert
        var action = () => errors.Throw();
        action.Should().Throw<AppException>()
            .WithMessage("No error details provided.");
    }

    [Fact]
    public void Throw_WithNullList_ShouldThrowAppException()
    {
        // Arrange
        List<IAppError> errors = null!;

        // Act & Assert
        var action = () => errors.Throw();
        action.Should().Throw<AppException>()
            .WithMessage("No error details provided.");
    }
}