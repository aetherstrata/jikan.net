using System;
using System.Collections.Generic;
using System.Linq;
using JikanDotNet.Consts;
using JikanDotNet.Extensions;
using JikanDotNet.Helpers;

namespace JikanDotNet.Query;

/// <summary>
/// Base class for query parameters
/// </summary>
internal class QueryParameter : IEquatable<QueryParameter>
{
    /// <summary>
    /// Query parameter name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Query parameter value
    /// </summary>
    public string Value { get; }

    // Internal instead of private for testing purpose
    internal QueryParameter(string name, string value)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        Guard.IsNotNull(value, nameof(value));
        Name = name;
        Value = value;
    }

    public override string ToString() => $"{Name}={Value}";

    #region Factory Methods

    internal static QueryParameter Category(ClubCategory category) => Enum("category", category);

    internal static QueryParameter EndDate(string date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;
        Guard.IsValidDate(date, nameof(date));
        return new QueryParameter("end_date", date);
    }

    internal static QueryParameter ExcludedGenres<T>(ICollection<T> excludedGenres) where T : struct, Enum
        => EnumCollection("genres_exclude", excludedGenres);

    internal static QueryParameter Filter<T>(T filter) where T : struct, Enum => Enum("filter", filter);

    internal static QueryParameter Gender(UserGender gender) => Enum("gender", gender);

    internal static QueryParameter Genres<T>(ICollection<T> genres) where T : struct, Enum
        => EnumCollection("genres", genres);

    internal static QueryParameter Kids(bool onlyKids)
    {
        return new QueryParameter("kids", onlyKids.ToStringLower());
    }

    internal static QueryParameter Letter(char? letter)
    {
        if (!letter.HasValue) return null;
        Guard.IsLetter(letter.Value, nameof(letter));
        return new QueryParameter("letter", letter.Value.ToString());
    }

    internal static QueryParameter Limit(int? limit)
    {
        if (!limit.HasValue) return null;
        Guard.IsGreaterThanZero(limit.Value, nameof(limit));
        Guard.IsLessThanOrEqual(limit.Value,ParameterConsts.MaximumPageSize, nameof(limit));
        return new QueryParameter("limit", limit.Value.ToString());
    }

    internal static QueryParameter Location(string location)
    {
        if (string.IsNullOrWhiteSpace(location)) return null;
        return new QueryParameter("location", location);
    }

    internal static QueryParameter Magazines(ICollection<long> ids)
    {
        if (ids.Count == 0) return null;
        return new QueryParameter("magazines", string.Join(",", ids));
    }

    internal static QueryParameter MaxAge(int? maxAge)
    {
        if (!maxAge.HasValue) return null;
        return new QueryParameter("max_age", maxAge.Value.ToString());
    }

    internal static QueryParameter MaxScore(int? maxScore)
    {
        if (!maxScore.HasValue) return null;
        Guard.IsLessThanOrEqual(maxScore.Value, 10, nameof(maxScore.Value));
        return new QueryParameter("max_score", maxScore.Value.ToString());
    }

    internal static QueryParameter MinAge(int? minAge)
    {
        if (!minAge.HasValue) return null;
        return new QueryParameter("min_age", minAge.Value.ToString());
    }

    internal static QueryParameter MinScore(int? minScore)
    {
        if (!minScore.HasValue) return null;
        Guard.IsGreaterThanOrEqual(minScore.Value, 0, nameof(minScore));
        return new QueryParameter("min_score", minScore.Value.ToString());
    }

    internal static QueryParameter OrderBy<T>(T orderEnum) where T : struct, Enum => Enum("order_by", orderEnum);

    internal static QueryParameter Page(int? page)
    {
        if (!page.HasValue) return null;
        Guard.IsGreaterThanZero(page.Value, nameof(page));
        return new QueryParameter("page", page.Value.ToString());
    }

    internal static QueryParameter Preliminary(bool includePreliminary)
    {
        return new QueryParameter("preliminary", includePreliminary.ToStringLower());
    }

    internal static QueryParameter Producers(ICollection<long> ids)
    {
        if (ids.Count == 0) return null;
        return new QueryParameter("producers", string.Join(",", ids));
    }

    internal static QueryParameter Query(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return null;
        return new QueryParameter("q", query);
    }

    internal static QueryParameter Rating<T>(T rating) where T : struct, Enum => Enum("rating", rating);

    internal static QueryParameter SafeForWork(bool excludeNsfw)
    {
        return new QueryParameter("sfw", excludeNsfw.ToStringLower());
    }

    internal static QueryParameter Sort(SortDirection sortDirection) => Enum("sort", sortDirection);

    internal static QueryParameter Spoiler(bool includeSpoiler)
    {
        return new QueryParameter("spoiler", includeSpoiler.ToStringLower());
    }

    internal static QueryParameter Spoilers(bool includeSpoilers)
    {
        return new QueryParameter("spoilers", includeSpoilers.ToStringLower());
    }

    internal static QueryParameter StartDate(string date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;
        Guard.IsValidDate(date, nameof(date));
        return new QueryParameter("start_date", date);
    }

    internal static QueryParameter Status<T>(T status) where T : struct, Enum => Enum("status", status);

    internal static QueryParameter Type<T>(T type) where T : struct, Enum => Enum("type", type);

    internal static QueryParameter Unapproved(bool includeUnapproved)
    {
        return new QueryParameter("unapproved", includeUnapproved.ToStringLower());
    }

    #endregion

    private static QueryParameter Enum<T>(string name, T value) where T : struct, Enum
    {
        Guard.IsValidEnum(value, nameof(value));
        return new QueryParameter(name, value.GetDescription());
    }

    private static QueryParameter EnumCollection<T>(string name, ICollection<T> value) where T : struct, Enum
    {
        if (value.Count == 0) return null;
        var ids = value.Select(e =>
        {
            Guard.IsValidEnum(e, nameof(e));
            return e.GetDescription();
        });
        return new QueryParameter(name, string.Join(",", ids));
    }

    #region Equality Members

    public bool Equals(QueryParameter other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is QueryParameter other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397) ^ Value.GetHashCode();
        }
    }

    public static IEqualityComparer<QueryParameter> NameComparer { get; } = new NameEqualityComparer();

    private sealed class NameEqualityComparer : IEqualityComparer<QueryParameter>
    {
        public bool Equals(QueryParameter x, QueryParameter y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(QueryParameter obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    #endregion
}