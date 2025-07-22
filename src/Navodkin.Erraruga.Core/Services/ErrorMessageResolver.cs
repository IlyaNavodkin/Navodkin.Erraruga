using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Dtos.Contracts;
using Navodkin.Erraruga.Core.Services.Contracts;

namespace Navodkin.Erraruga.Core.Services;

public class ErrorMessageResolver : IErrorMessageResolver
{
    public ICollection<IErrorRule> BaseRules { get; } = new List<IErrorRule>();
    private ICollection<IErrorRule> CustomRules { get; } = new List<IErrorRule>();

    public ErrorMessageResolver WithDefaultRule(IErrorRule errorRule)
    {
        BaseRules.Add(errorRule);

        return this;
    }

    public ErrorMessageResolver WithRule(IErrorRule errorRule)
    {
        CustomRules.Add(errorRule);

        return this;
    }

    public string Resolve(IAppError error, bool baseRulesForceUsed = false)
    {
        if (error == null) throw new ArgumentNullException(nameof(error));

        var context = error.Context;

        if (!baseRulesForceUsed)
        {
            var customRule = CustomRules.FirstOrDefault
                (r => r.Key == (error.Code, context));

            if (customRule != null)
            {
                var result = customRule.Handler(error);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return result;
                }
            }
        }

        var baseRule = BaseRules
            .FirstOrDefault(r => r.Code == error.Code);

        if (baseRule != null)
        {
            return baseRule.Handler(error);
        }

        return LowlevelHandle(error, context);
    }

    public string LowlevelHandle(IAppError error, string? context)
    {
        var builder = new StringBuilder("Unknown error.\n");

        if (error == null) return builder.ToString();

        builder
            .Append(error.Code)
            .Append(": ")
            .Append(error.Message)
            .Append("\n");

        if (!string.IsNullOrWhiteSpace(context))
            builder.Append($"Context: {context}\n");

        if (error.Metadata.Count > 0)
        {
            builder.Append("Metadata:\n");
            foreach (var data in error.Metadata)
            {
                var key = data.Key;
                var value = data.Value?.ToString();
                builder.Append($"{key}: {value}\n");
            }
        }

        return builder.ToString();
    }
}
