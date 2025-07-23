using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Example.WPF.Services;
using Example.WPF.Constants;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Extensions;

namespace Example.WPF.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ErrorService _errorService;
    private string _currentError = string.Empty;
    private string _currentErrorJson = string.Empty;

    public MainViewModel()
    {
        _errorService = new ErrorService();
        
        TestValidationErrorCommand = new RelayCommand(TestValidationError);
        TestNotFoundErrorCommand = new RelayCommand(TestNotFoundError);
        TestPermissionErrorCommand = new RelayCommand(TestPermissionError);
        TestNetworkErrorCommand = new RelayCommand(TestNetworkError);
        TestDatabaseErrorCommand = new RelayCommand(TestDatabaseError);
        TestConfigurationErrorCommand = new RelayCommand(TestConfigurationError);
        TestCustomUserErrorCommand = new RelayCommand(TestCustomUserError);
        TestCustomSystemErrorCommand = new RelayCommand(TestCustomSystemError);
        TestUnknownErrorCommand = new RelayCommand(TestUnknownError);
        TestExceptionErrorCommand = new RelayCommand(TestExceptionError);
        TestRuntimeContextCommand = new RelayCommand(TestRuntimeContext);
        ClearErrorCommand = new RelayCommand(ClearError);
    }

    public string CurrentError
    {
        get => _currentError;
        set
        {
            _currentError = value;
            OnPropertyChanged();
        }
    }

    public string CurrentErrorJson
    {
        get => _currentErrorJson;
        set
        {
            _currentErrorJson = value;
            OnPropertyChanged();
        }
    }

    public ICommand TestValidationErrorCommand { get; }
    public ICommand TestNotFoundErrorCommand { get; }
    public ICommand TestPermissionErrorCommand { get; }
    public ICommand TestNetworkErrorCommand { get; }
    public ICommand TestDatabaseErrorCommand { get; }
    public ICommand TestConfigurationErrorCommand { get; }
    public ICommand TestCustomUserErrorCommand { get; }
    public ICommand TestCustomSystemErrorCommand { get; }
    public ICommand TestUnknownErrorCommand { get; }
    public ICommand TestExceptionErrorCommand { get; }
    public ICommand TestRuntimeContextCommand { get; }
    public ICommand ClearErrorCommand { get; }

    private void TestValidationError()
    {
        var error = _errorService.CreateValidationError("Email", "Invalid email format");
        SetError(error);
    }

    private void TestNotFoundError()
    {
        var error = _errorService.CreateNotFoundError("User");
        SetError(error);
    }

    private void TestPermissionError()
    {
        var error = _errorService.CreatePermissionError("user123");
        SetError(error);
    }

    private void TestNetworkError()
    {
        var error = _errorService.CreateNetworkError("Data loading", 5);
        SetError(error);
    }

    private void TestDatabaseError()
    {
        var error = _errorService.CreateDatabaseError("SELECT * FROM Users", "Server=localhost;Database=TestDB");
        SetError(error);
    }

    private void TestConfigurationError()
    {
        var error = _errorService.CreateConfigurationError("ConnectionString", "invalid_connection");
        SetError(error);
    }

    private void TestCustomUserError()
    {
        var error = _errorService.CreateCustomError("User not authorized", "UserContext", "user123", "session456");
        SetError(error);
    }

    private void TestCustomSystemError()
    {
        var error = _errorService.CreateCustomError("Database connection failed", "SystemContext", "system", "sys789");
        SetError(error);
    }

    private void TestUnknownError()
    {
        var error = _errorService.CreateUnknownError();
        SetError(error);
    }

    private void TestExceptionError()
    {
        try
        {
            // Simulate an exception
            throw new InvalidOperationException("This is a simulated exception for testing");
        }
        catch (Exception ex)
        {
            var error = _errorService.CreateUnknownError(ex);
            SetError(error);
        }
    }

    private void TestRuntimeContext()
    {
        // Demonstrate GetRuntimeContext usage
        var context = this.GetRuntimeContext();
        var error = new AppError(ErrorCodes.CustomError, "Runtime context demonstration", context);
        error.AppendMetadata(MetadataKeys.Timestamp, DateTime.Now);
        error.AppendMetadata(MetadataKeys.ErrorLevel, "Info");
        error.AppendMetadata(MetadataKeys.Category, "Demo");
        error.AppendMetadata("RuntimeContext", context);
        SetError(error);
    }

    private void ClearError()
    {
        CurrentError = string.Empty;
        CurrentErrorJson = string.Empty;
    }

    private void SetError(AppError error)
    {
        CurrentError = _errorService.ResolveError(error);
        CurrentErrorJson = JsonSerializer.Serialize(error, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Простая реализация ICommand для WPF
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();
} 