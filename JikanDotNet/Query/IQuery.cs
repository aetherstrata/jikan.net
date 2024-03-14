namespace JikanDotNet.Query;

/// <summary>
/// Representation of an endpoint query string
/// </summary>
internal interface IQuery
{
    /// <summary>
    /// Get the query to execute
    /// </summary>
    /// <returns>The full query representation</returns>
    string GetQuery();
}