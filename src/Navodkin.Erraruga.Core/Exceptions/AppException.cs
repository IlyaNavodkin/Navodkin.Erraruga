using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Navodkin.Erraruga.Core.Dtos.Contracts;

namespace Navodkin.Erraruga.Core.Exceptions;

public class AppException : Exception
{
    public ICollection<IAppError> Errors { get; private set; }

    public AppException(ICollection<IAppError> errors)
    {
        Errors = errors;
    }

    public AppException(IAppError error)
    {
        Errors = new Collection<IAppError> { error };
    }

    public override string Message => CreateMessage(Errors);

    private string CreateMessage(ICollection<IAppError> errors)
    {
        if (errors != null && errors.Count > 0)
        {
            return "App errors: " +
                string.Join("; ", errors.Select(e => $"({e.Code}): {e.Message}"));
        }

        return "No error details provided.";
    }
}
