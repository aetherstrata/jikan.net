using System;
using System.Diagnostics.CodeAnalysis;
using JikanDotNet.Helpers;

namespace JikanDotNet.Query;

/// <summary>
/// Base class for query parameters
/// </summary>
internal abstract class QueryParameter : IEquatable<QueryParameter>
{
    internal string Name { get; }

    protected QueryParameter(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    #region Equality Members

    public bool Equals(QueryParameter other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((QueryParameter)obj);
    }

    public override int GetHashCode() => Name.GetHashCode();

    public static bool operator ==(QueryParameter left, QueryParameter right) => Equals(left, right);

    public static bool operator !=(QueryParameter left, QueryParameter right) => !Equals(left, right);

    #endregion
}