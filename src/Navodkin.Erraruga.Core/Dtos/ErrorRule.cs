using System;
using Navodkin.Erraruga.Core.Dtos.Contracts;

namespace Navodkin.Erraruga.Core.Dtos;

public class ErrorRule : IErrorRule
{
    public string Code { get; }
    public string Context { get; }
    public Func<IAppError, string> Handler { get; }

    public ErrorRule(string code, Func<IAppError, string> handler, string context = null)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        Context = context;
    }

    public (string Code, string Context) Key => (Code, Context);
}
