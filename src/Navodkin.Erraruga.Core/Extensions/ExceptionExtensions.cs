using System;
using Navodkin.Erraruga.Core.Constants;
using Navodkin.Erraruga.Core.Dtos;

namespace Navodkin.Erraruga.Core.Extensions;

public static class ExceptionExtensions
{
    private static AppError BuildExceptionError(Exception exception,
        string context,
        string errorCode,
        string additionalMessage)
    {
        var errorMessage = $"{exception.Message}";
        var stackTrace = $"{exception.StackTrace}";

        var error = new AppError(errorCode, errorMessage, context);

        error.AppendMetadata(DefaultErrorFields.StackTrace, stackTrace);

        if (additionalMessage != null)
            error.AppendMetadata(DefaultErrorFields.AdditionalMessage, additionalMessage);

        return error;
    }
}


