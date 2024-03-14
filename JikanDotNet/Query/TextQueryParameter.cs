namespace JikanDotNet.Query;

/// <summary>
/// A text query parameter
/// </summary>
/// <example>?sfw</example>
internal sealed class TextQueryParameter(string paramName) : QueryParameter(paramName)
{
    public override string ToString() => Name;
}