namespace JikanDotNet.Extensions;

internal static class BoolExtensions
{
    internal static string ToStringLower(this bool val) => val switch
    {
        true => "true",
        false => "false"
    };
}