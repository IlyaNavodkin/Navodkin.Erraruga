using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Services;
using Navodkin.Erraruga.Core.Extensions;
using Example.WPF.Constants;

namespace Example.WPF.Services;

public class ErrorService
{
    private readonly ErrorMessageResolver _errorResolver;

    public ErrorService()
    {
        _errorResolver = new ErrorMessageResolver()
            .WithDefaultRule(new ErrorRule(ErrorCodes.ValidationError, error => $"Validation error: {error.Message}"))
            .WithDefaultRule(new ErrorRule(ErrorCodes.NotFound, error => $"Resource not found: {error.Context}"))
            .WithDefaultRule(new ErrorRule(ErrorCodes.PermissionDenied, error => "Access denied"))
            .WithDefaultRule(new ErrorRule(ErrorCodes.NetworkError, error => $"Network error: {error.Message}"))
            .WithDefaultRule(new ErrorRule(ErrorCodes.DatabaseError, error => $"Database error: {error.Message}"))
            .WithDefaultRule(new ErrorRule(ErrorCodes.ConfigurationError, error => $"Configuration error: {error.Message}"))
            .WithRule(new ErrorRule(ErrorCodes.CustomError, error => $"User error: {error.Message}", "UserContext"))
            .WithRule(new ErrorRule(ErrorCodes.CustomError, error => $"System error: {error.Message}", "SystemContext"));
    }

    public string ResolveError(AppError error)
    {
        return _errorResolver.Resolve(error);
    }

    public AppError CreateValidationError(string field, string message)
    {
        var error = new AppError(ErrorCodes.ValidationError, message, this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Field, field);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Warning");
        error.AppendMetadata(MetadataKeys.Category, "Validation");
        return error;
    }

    public AppError CreateNotFoundError(string resource)
    {
        var error = new AppError(ErrorCodes.NotFound, $"Resource '{resource}' not found", this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Target, resource);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Info");
        error.AppendMetadata(MetadataKeys.Category, "Resource");
        return error;
    }

    public AppError CreatePermissionError(string userId = null)
    {
        var error = new AppError(ErrorCodes.PermissionDenied, "Insufficient permissions for operation", this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Error");
        error.AppendMetadata(MetadataKeys.Category, "Security");
        if (!string.IsNullOrEmpty(userId))
        {
            error.AppendMetadata(MetadataKeys.UserId, userId);
        }
        return error;
    }

    public AppError CreateNetworkError(string operation, int retryCount = 3)
    {
        var error = new AppError(ErrorCodes.NetworkError, $"Error during operation: {operation}", this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Operation, operation);
        error.AppendMetadata(MetadataKeys.RetryCount, retryCount);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Error");
        error.AppendMetadata(MetadataKeys.Category, "Network");
        error.AppendMetadata(MetadataKeys.Environment, "Production");
        return error;
    }

    public AppError CreateDatabaseError(string operation, string connectionString = null)
    {
        var error = new AppError(ErrorCodes.DatabaseError, $"Database operation failed: {operation}", this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Operation, operation);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Critical");
        error.AppendMetadata(MetadataKeys.Category, "Database");
        error.AppendMetadata(MetadataKeys.Source, "SQL Server");
        if (!string.IsNullOrEmpty(connectionString))
        {
            error.AppendMetadata("ConnectionString", connectionString);
        }
        return error;
    }

    public AppError CreateConfigurationError(string setting, string value = null)
    {
        var error = new AppError(ErrorCodes.ConfigurationError, $"Configuration error for setting: {setting}", this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Field, setting);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Error");
        error.AppendMetadata(MetadataKeys.Category, "Configuration");
        error.AppendMetadata(MetadataKeys.Version, "1.0.0");
        if (!string.IsNullOrEmpty(value))
        {
            error.AppendMetadata(MetadataKeys.Target, value);
        }
        return error;
    }

    public AppError CreateCustomError(string message, string context, string userId = null, string sessionId = null)
    {
        var error = new AppError(ErrorCodes.CustomError, message, context);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.Context, context);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Info");
        error.AppendMetadata(MetadataKeys.Category, "Custom");
        if (!string.IsNullOrEmpty(userId))
        {
            error.AppendMetadata(MetadataKeys.UserId, userId);
        }
        if (!string.IsNullOrEmpty(sessionId))
        {
            error.AppendMetadata(MetadataKeys.SessionId, sessionId);
        }
        return error;
    }

    public AppError CreateUnknownError(Exception exception = null)
    {
        var error = new AppError(ErrorCodes.UnknownError, "Unknown error occurred", this.GetRuntimeContext());
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Critical");
        error.AppendMetadata(MetadataKeys.Category, "System");
        if (exception != null)
        {
            error.AppendMetadata(MetadataKeys.StackTrace, exception.StackTrace);
            error.AppendMetadata(MetadataKeys.Source, exception.Source);
        }
        return error;
    }
} 