using System.Collections.Generic;
using Navodkin.Erraruga.Core.Dtos.Contracts;
using Navodkin.Erraruga.Core.Exceptions;

namespace Navodkin.Erraruga.Core.Extensions;

public static class AppErrorExtensions
{
    public static void Throw(this IAppError error)
        => throw new AppException(error);

    public static void Throw(this List<IAppError> errors)
        => throw new AppException(errors);
}
