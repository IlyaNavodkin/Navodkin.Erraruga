# Navodkin.Erraruga

<div align="center">

![Navodkin.Erraruga Logo](images/logo.png)

</div>

> **Simple error handling library for .NET applications**

---

## Overview

Navodkin.Erraruga is a simple and extensible error handling library for .NET, designed to simplify error management, provide rich error context, and improve debugging and user experience in your applications.

https://www.nuget.org/packages/Navodkin.Erraruga.Core
---

## Features

- Centralized error representation
- Customizable error rules and messages
- Easy integration with .NET projects
- Extensible contracts for advanced scenarios
- Rich error context support
- Custom error rules and message resolution

---

## Installation

Install via NuGet:

```shell
Install-Package Navodkin.Erraruga.Core
```

---

## Quick Start Example

Below is a minimal example of how to use Navodkin.Erraruga in a WPF application:

```csharp
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Services;

// Create an error
var error = new AppError("E001", "An unexpected error occurred.");

// Resolve error message
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("E001", e => "An unexpected error occurred. Please try again."))
    .Build();

string userMessage = resolver.Resolve(error);
Console.WriteLine(userMessage);
// Output: An unexpected error occurred. Please try again.
```

See more in [example/Example.WPF](./example/Example.WPF/).

---

## ErrorMessageResolverBuilder

The `ErrorMessageResolverBuilder` provides a fluent API for configuring error message resolution:

```csharp
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("E001", e => "Default error message"))
    .WithDefaultRule(new ErrorRule("VAL001", e => "Validation error"))
    .WithRule(new ErrorRule("VAL001", e => "Specific validation error"))
    .Build();
```

### Builder Methods

- `WithDefaultRule(ErrorRule rule)` - Adds a default rule that will be used when no custom rule matches
- `WithRule(ErrorRule rule)` - Adds a specific rule that takes precedence over default rules
- `Build()` - Creates and returns the configured `IErrorMessageResolver`

---

## Rules and Context System

### Understanding WithDefaultRule vs WithRule

The library provides two types of rules with different priorities:

- **WithDefaultRule**: Creates fallback rules that are used when no specific rule matches
- **WithRule**: Creates specific rules that take precedence over default rules

### Rule Priority and Context Matching

Rules are matched based on error code and context:
1. **Specific Rule** (WithRule): Matches by code + context
2. **Default Rule** (WithDefaultRule): Matches by code only (fallback)

### Creating Rules

```csharp
// Simple default rule
var defaultRule = new ErrorRule("E001", error => "An unexpected error occurred.");

// Rule with context (for specific scenarios)
var contextRule = new ErrorRule("DB001", error => "Database error occurred.", "Connection");

// Rule with dynamic message based on error properties
var dynamicRule = new ErrorRule("VAL001", error => 
{
    var field = error.Metadata.GetValueOrDefault("Field", "Unknown");
    return $"Validation failed for field: {field}";
});
```

### Configuring Resolver with Both Rule Types

```csharp
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("E001", e => "An unexpected error occurred."))
    .WithDefaultRule(new ErrorRule("DB001", e => "Database connection failed."))
    .WithDefaultRule(new ErrorRule("VAL001", e => "Validation error occurred."))
    .WithRule(new ErrorRule("DB001", e => "Connection timeout in user login", "UserLogin"))
    .WithRule(new ErrorRule("VAL001", e => "Email validation failed", "Registration"))
    .Build();

// Usage examples
var generalError = new AppError("E001", "System error");
var dbError = new AppError("DB001", "Connection failed", "UserLogin");
var valError = new AppError("VAL001", "Validation failed", "Registration");

Console.WriteLine(resolver.Resolve(generalError)); // Uses default rule
Console.WriteLine(resolver.Resolve(dbError));      // Uses specific rule with context
Console.WriteLine(resolver.Resolve(valError));     // Uses specific rule with context
```

---

## Custom Error Examples

### Creating Custom Errors

```csharp
// Basic custom error
var validationError = new AppError("VAL001", "Validation failed");

// Error with metadata
var databaseError = new AppError("DB001", "Database connection failed");
databaseError.AppendMetadata("ConnectionString", "Server=localhost;Database=MyApp");
databaseError.AppendMetadata("Timeout", 30);
databaseError.AppendMetadata("RetryCount", 3);

// Error with context (enables specific rule matching)
var contextError = new AppError("E001", "An error occurred", "UserLogin");
```

### Working with Error Metadata

```csharp
// Add metadata to existing error
error.AppendMetadata("UserId", 12345);
error.AppendMetadata("Operation", "UserLogin");
error.AppendMetadata("Timestamp", DateTime.UtcNow);

// Retrieve metadata values
if (error.Metadata.TryGetValue("UserId", out var userId))
{
    Console.WriteLine($"Error occurred for user: {userId}");
}
```

### Example with Output

```csharp
var error = new AppError("VAL001", "Email validation failed");
error.AppendMetadata("Field", "Email");
error.AppendMetadata("Value", "invalid-email");

var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("VAL001", e => 
    {
        var field = e.Metadata.GetValueOrDefault("Field", "Unknown");
        var value = e.Metadata.GetValueOrDefault("Value", "N/A");
        return $"Validation failed for {field}: '{value}' is not a valid email address.";
    }))
    .Build();

string result = resolver.Resolve(error);
Console.WriteLine(result);
// Output: Validation failed for Email: 'invalid-email' is not a valid email address.
```

---

## Custom Error Rules

### Creating Custom Rules

```csharp
// Using ErrorRule class
var validationRule = new ErrorRule("VAL001", error => 
{
    var field = error.Metadata.GetValueOrDefault("Field", "Unknown");
    var value = error.Metadata.GetValueOrDefault("Value", "N/A");
    
    return $"Validation error in field '{field}' with value '{value}'. Please check your input.";
});

// Using resolver with custom rules
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(validationRule)
    .Build();

var error = new AppError("VAL001", "Validation failed");
error.AppendMetadata("Field", "Email");
error.AppendMetadata("Value", "test@");

string result = resolver.Resolve(error);
Console.WriteLine(result);
// Output: Validation error in field 'Email' with value 'test@'. Please check your input.
```

### Advanced Rule Examples with Context

```csharp
// Database error rule with retry logic
var databaseRule = new ErrorRule("DB001", error =>
{
    var retryCount = error.Metadata.GetValueOrDefault("RetryCount", 0);
    
    if (retryCount > 0)
    {
        return $"Database operation failed after {retryCount} retries. Please try again later.";
    }
    
    return "Database connection failed. Please check your network connection.";
});

// Specific database rule for user login context
var loginDbRule = new ErrorRule("DB001", error =>
{
    var retryCount = error.Metadata.GetValueOrDefault("RetryCount", 0);
    return retryCount > 0 
        ? $"Login failed after {retryCount} attempts. Please check your credentials and try again."
        : "Unable to verify login credentials. Please try again.";
}, "UserLogin");

// Localized error rule
var localizedMessages = new Dictionary<string, string>
{
    ["E001"] = "Произошла непредвиденная ошибка",
    ["VAL001"] = "Ошибка валидации данных",
    ["DB001"] = "Ошибка подключения к базе данных"
};

var localizedRule = new ErrorRule("E001", error => 
{
    return localizedMessages.GetValueOrDefault(error.Code, "Unknown error");
});

// Usage example with both rule types
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(databaseRule)
    .WithRule(loginDbRule)
    .WithDefaultRule(localizedRule)
    .Build();

// Test different scenarios
var generalDbError = new AppError("DB001", "Connection timeout");
generalDbError.AppendMetadata("RetryCount", 3);

var loginDbError = new AppError("DB001", "Connection timeout", "UserLogin");
loginDbError.AppendMetadata("RetryCount", 2);

Console.WriteLine(resolver.Resolve(generalDbError)); // Uses default rule
Console.WriteLine(resolver.Resolve(loginDbError));   // Uses specific rule with context

// Output:
// Database operation failed after 3 retries. Please try again later.
// Login failed after 2 attempts. Please check your credentials and try again.
```

---

## Interfaces and Extensibility

### IAppError Interface

```csharp
public interface IAppError
{
    string Code { get; set; }
    string Context { get; set; }
    string Message { get; set; }
    Dictionary<string, object> Metadata { get; set; }
    void AppendMetadata(string key, object value);
}
```

### IErrorRule Interface

```csharp
public interface IErrorRule
{
    string Code { get; }
    string Context { get; }
    Func<IAppError, string> Handler { get; }
    (string Code, string Context) Key { get; }
}
```

### IErrorMessageResolver Interface

```csharp
public interface IErrorMessageResolver
{
    string Resolve(IAppError error, bool baseRulesForceUsed = false);
    ErrorMessageResolver WithDefaultRule(IErrorRule errorRule);
    ErrorMessageResolver WithRule(IErrorRule errorRule);
}
```

### Context System Explanation

The context system allows you to create different error messages for the same error code based on where the error occurs:

- **Without Context**: Rules match only by error code
- **With Context**: Rules match by both error code and context string

**Example Context Usage:**
```csharp
// Same error code, different contexts
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("DB001", e => "General database error"))
    .WithRule(new ErrorRule("DB001", e => "Login database error", "UserLogin"))
    .WithRule(new ErrorRule("DB001", e => "Payment database error", "Payment"))
    .Build();

// Different results based on context
var generalError = new AppError("DB001", "Connection failed");
var loginError = new AppError("DB001", "Connection failed", "UserLogin");
var paymentError = new AppError("DB001", "Connection failed", "Payment");

Console.WriteLine(resolver.Resolve(generalError)); // "General database error"
Console.WriteLine(resolver.Resolve(loginError));   // "Login database error"
Console.WriteLine(resolver.Resolve(paymentError)); // "Payment database error"
```

---

## Advanced Usage Examples

### Error Service Integration

```csharp
public class ErrorService
{
    private readonly IErrorMessageResolver _resolver;

    public ErrorService()
    {
        _resolver = new ErrorMessageResolverBuilder()
            .WithDefaultRule(new ErrorRule("VAL001", e => 
            {
                var field = e.Metadata.GetValueOrDefault("Field", "Unknown");
                return $"Validation error in field '{field}'. Please check your input.";
            }))
            .WithDefaultRule(new ErrorRule("DB001", e => 
            {
                var retryCount = e.Metadata.GetValueOrDefault("RetryCount", 0);
                return retryCount > 0 
                    ? $"Database operation failed after {retryCount} retries." 
                    : "Database connection failed.";
            }))
            .WithRule(new ErrorRule("DB001", e => 
            {
                var retryCount = e.Metadata.GetValueOrDefault("RetryCount", 0);
                return retryCount > 0 
                    ? $"Login failed after {retryCount} attempts." 
                    : "Unable to verify login credentials.";
            }, "UserLogin"))
            .Build();
    }

    public void HandleError(AppError error)
    {
        var userMessage = _resolver.Resolve(error);
        
        // Log error with metadata
        LogError(error);
        
        // Show user-friendly message
        ShowUserMessage(userMessage);
    }

    private void LogError(AppError error)
    {
        var logMessage = $"Error {error.Code}: {error.Message}";
        if (error.Metadata.Any())
        {
            logMessage += $" Metadata: {string.Join(", ", error.Metadata.Select(kv => $"{kv.Key}={kv.Value}"))}";
        }
        
        Console.WriteLine(logMessage);
    }
}

// Usage
var errorService = new ErrorService();

// Test default rule
var validationError = new AppError("VAL001", "Validation failed");
validationError.AppendMetadata("Field", "Email");

// Test specific rule with context
var loginError = new AppError("DB001", "Connection failed", "UserLogin");
loginError.AppendMetadata("RetryCount", 3);

errorService.HandleError(validationError);
errorService.HandleError(loginError);

// Output:
// Log: Error VAL001: Validation failed Metadata: Field=Email
// User message: Validation error in field 'Email'. Please check your input.
// Log: Error DB001: Connection failed Metadata: RetryCount=3
// User message: Login failed after 3 attempts.
```

### Exception Integration

```csharp
try
{
    // Your business logic
    throw new InvalidOperationException("Database connection failed");
}
catch (Exception ex)
{
    var appError = ex.ToAppError("DB001", "Database operation failed");
    appError.AppendMetadata("ConnectionString", "Server=localhost");
    appError.AppendMetadata("RetryCount", 3);
    
    var resolver = new ErrorMessageResolverBuilder()
        .WithDefaultRule(new ErrorRule("DB001", e => 
        {
            var retryCount = e.Metadata.GetValueOrDefault("RetryCount", 0);
            return retryCount > 0 
                ? $"Database operation failed after {retryCount} retries." 
                : "Database connection failed.";
        }))
        .Build();
        
    var userMessage = resolver.Resolve(appError);
    Console.WriteLine(userMessage);
    // Output: Database operation failed after 3 retries.
}
```

### Complex Example with Multiple Rules

```csharp
// Create resolver with multiple rules
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("E001", e => "An unexpected error occurred."))
    .WithDefaultRule(new ErrorRule("VAL001", e => 
    {
        var field = e.Metadata.GetValueOrDefault("Field", "Unknown");
        var value = e.Metadata.GetValueOrDefault("Value", "N/A");
        return $"Validation failed for {field}: '{value}' is invalid.";
    }))
    .WithDefaultRule(new ErrorRule("DB001", e => 
    {
        var retryCount = e.Metadata.GetValueOrDefault("RetryCount", 0);
        return retryCount > 0 
            ? $"Database operation failed after {retryCount} retries." 
            : "Database connection failed.";
    }))
    .Build();

// Test different error types
var errors = new[]
{
    new AppError("E001", "System error"),
    new AppError("VAL001", "Validation error") { Metadata = { ["Field"] = "Email", ["Value"] = "invalid" } },
    new AppError("DB001", "Database error") { Metadata = { ["RetryCount"] = 2 } }
};

foreach (var error in errors)
{
    var result = resolver.Resolve(error);
    Console.WriteLine($"Error {error.Code}: {result}");
}

// Output:
// Error E001: An unexpected error occurred.
// Error VAL001: Validation failed for Email: 'invalid' is invalid.
// Error DB001: Database operation failed after 2 retries.
```

---

## Project Structure

```
Navodkin.Erraruga/
├── src/
│   └── Navodkin.Erraruga.Core/   # Core library
│       ├── Dtos/                  # Data transfer objects
│       ├── Contracts/             # Interfaces and contracts
│       ├── Services/              # Core services
│       ├── Extensions/            # Extension methods
│       └── Exceptions/            # Custom exceptions
├── example/
│   └── Example.WPF/              # WPF usage example
├── tests/                        # Unit and integration tests
```

- **src/Navodkin.Erraruga.Core/**: Main library source code
- **example/Example.WPF/**: Example WPF application demonstrating usage
- **tests/**: Unit and integration tests

---

## License

This project is licensed under the MIT License. See [LICENSE.md](./LICENSE.md) for details.
