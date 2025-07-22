using Navodkin.Erraruga.Core.Dtos;
using System;

namespace Navodkin.Erraruga.Core.Dtos.Contracts;

public interface IErrorRule
{
    string Code { get; }
    string Context { get; }
    Func<IAppError, string> Handler { get; }
    (string Code, string Context) Key { get; }
}