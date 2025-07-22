using Navodkin.Erraruga.Core.Dtos;
using Navodkin.Erraruga.Core.Dtos.Contracts;
using System.Collections.Generic;

namespace Navodkin.Erraruga.Core.Services.Contracts;

public interface IErrorMessageResolver
{
    ICollection<IErrorRule> BaseRules { get; }

    string LowlevelHandle(IAppError error, string context);
    string Resolve(IAppError error, bool baseRulesForceUsed = false);
    ErrorMessageResolver WithDefaultRule(IErrorRule errorRule);
    ErrorMessageResolver WithRule(IErrorRule errorRule);
}