using System;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Services.Contracts;

namespace Navodkin.Erraruga.Core.Services;

public class ErrorMessageResolverBuilder
{
    public IErrorMessageResolver Instance { get; private set; }

    public ErrorMessageResolverBuilder()
    {
        Instance = new ErrorMessageResolver();
    }

    public ErrorMessageResolverBuilder WithDefaultRule(
        ErrorRule errorRule
    )
    {
        Instance.WithDefaultRule(errorRule);

        return this;
    }

    public IErrorMessageResolver Build() => Instance;
}
