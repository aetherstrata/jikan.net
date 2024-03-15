using System;

namespace JikanDotNet.Exceptions;

/// <summary>
/// Exception class thrown when a parameter with the same name already exists in a query
/// </summary>
public class JikanDuplicateParameterException : Exception
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; }

    internal JikanDuplicateParameterException(string name, string message = null) : base(message)
    {
        Name = name;
    }
}