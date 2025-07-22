using System.Runtime.CompilerServices;

namespace Navodkin.Erraruga.Core.Extensions;

public static class ObjectExtensions
{
    public static string GetRuntimeContext(
        this object obj,
        [CallerMemberName] string caller = ""
        )
    {
        return $"{obj.GetType().Name}.{caller}";
    }
}


