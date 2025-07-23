namespace Example.WPF.Constants;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string PermissionDenied = "PERMISSION_DENIED";
    public const string NetworkError = "NETWORK_ERROR";
    public const string CustomError = "CUSTOM_ERROR";
    public const string UnknownError = "UNKNOWN_ERROR";
    public const string DatabaseError = "DATABASE_ERROR";
    public const string ConfigurationError = "CONFIGURATION_ERROR";
}

public static class MetadataKeys
{
    public const string Field = "Field";
    public const string Timestamp = "Timestamp";
    public const string Operation = "Operation";
    public const string RetryCount = "RetryCount";
    public const string UserId = "UserId";
    public const string SessionId = "SessionId";
    public const string RequestId = "RequestId";
    public const string Environment = "Environment";
    public const string Version = "Version";
    public const string StackTrace = "StackTrace";
    public const string ErrorLevel = "ErrorLevel";
    public const string Category = "Category";
    public const string Source = "Source";
    public const string Target = "Target";
    public const string Context = "Context";
} 