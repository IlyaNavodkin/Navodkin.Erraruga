using System.Collections.Generic;

namespace Navodkin.Erraruga.Core.Dtos.Contracts;

public interface IAppError
{
    string Code { get; set; }
    string Context { get; set; }
    string Message { get; set; }
    Dictionary<string, object> Metadata { get; set; }

    void AppendMetadata(string key, object value);
}