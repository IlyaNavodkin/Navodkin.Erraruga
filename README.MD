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
    .Build();
```

### Builder Methods

- `WithDefaultRule(ErrorRule rule)` - Adds a default rule that will be used when no custom rule matches
- `Build()` - Creates and returns the configured `IErrorMessageResolver`

---

## Default Rules and Configuration

### Creating Default Rules

```csharp
// Simple default rule
var defaultRule = new ErrorRule("E001", error => "An unexpected error occurred.");

// Rule with context
var contextRule = new ErrorRule("DB001", error => "Database error occurred.", "Connection");

// Rule with dynamic message based on error properties
var dynamicRule = new ErrorRule("VAL001", error => 
{
    var field = error.Metadata.GetValueOrDefault("Field", "Unknown");
    return $"Validation failed for field: {field}";
});
```

### Configuring Resolver with Default Rules

```csharp
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(new ErrorRule("E001", e => "An unexpected error occurred."))
    .WithDefaultRule(new ErrorRule("DB001", e => "Database connection failed."))
    .WithDefaultRule(new ErrorRule("VAL001", e => "Validation error occurred."))
    .Build();

// Usage
var error = new AppError("E001", "System error");
string result = resolver.Resolve(error);
Console.WriteLine(result);
// Output: An unexpected error occurred.
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

// Error with context
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

### Advanced Rule Examples

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

// Usage example
var resolver = new ErrorMessageResolverBuilder()
    .WithDefaultRule(databaseRule)
    .WithDefaultRule(localizedRule)
    .Build();

var dbError = new AppError("DB001", "Connection timeout");
dbError.AppendMetadata("RetryCount", 3);

string result = resolver.Resolve(dbError);
Console.WriteLine(result);
// Output: Database operation failed after 3 retries. Please try again later.
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
var error = new AppError("VAL001", "Validation failed");
error.AppendMetadata("Field", "Email");

errorService.HandleError(error);
// Log: Error VAL001: Validation failed Metadata: Field=Email
// User message: Validation error in field 'Email'. Please check your input.
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
