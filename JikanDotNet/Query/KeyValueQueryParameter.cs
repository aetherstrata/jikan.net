using JikanDotNet.Helpers;

namespace JikanDotNet.Query;

/// <summary>
/// A key-value query parameter
/// </summary>
/// <example>?page=3</example>
internal sealed class KeyValueQueryParameter : QueryParameter
{
    internal string Value { get; }

    public KeyValueQueryParameter(string name, string paramValue) : base(name)
    {
        Guard.IsNotNullOrWhiteSpace(paramValue, nameof(paramValue));
        Value = paramValue;
    }

    public override string ToString() => $"{Name}={Value}";
}