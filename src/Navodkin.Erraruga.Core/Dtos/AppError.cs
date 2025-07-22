using System;
using System.Collections.Generic;
using System.Linq;
using Navodkin.Erraruga.Core.Dtos.Contracts;

namespace Navodkin.Erraruga.Core.Dtos;

public class AppError : IAppError
{
    public AppError(string code, Dictionary<string, object> metadata)
    {
        Code = code;
        Metadata = metadata;
    }

    public AppError(string errorCode, string message, string context)
    {
        Message = message;
        Code = errorCode;
        Context = context;
    }

    public AppError(string code)
    {
        Code = code;
    }

    public AppError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; set; }
    public string Context { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = [];

    public void AppendMetadata(string key, object value)
    {
        Metadata[key] = value;
    }
}
