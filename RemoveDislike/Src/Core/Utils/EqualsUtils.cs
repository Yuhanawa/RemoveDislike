#nullable enable
namespace RemoveDislike.Core.Utils;

public static class EqualsUtils
{
    public static bool? RtBoolOrNull(object? origin,object? target) => origin == null ? null : origin == target;
    public static bool? StrEqRtBoolOrNull(this string? origin,string? target) => origin?.Equals(target);

    public static bool? ObjToStrEqRtBoolOrNull(this object? origin, object? target) => 
        origin?.ToString() == null ? null:target?.ToString() == null ? null : origin.ToString()!.StrEqRtBoolOrNull(target.ToString());
}