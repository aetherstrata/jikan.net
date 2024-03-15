using System.Collections.Generic;
using JikanDotNet.Interfaces;
using JikanDotNet.Query;

namespace JikanDotNet;

/// <summary>
/// Model class of search configuration for advanced person search.
/// </summary>
public class CharacterSearchConfig : ISearchConfig
{
	/// <summary>
	/// Index of page folding 50 records of top ranging (e.g. 1 will return first 50 records, 2 will return record from 51 to 100 etc.)
	/// </summary>
	public int? Page { get; set; }
	
	/// <summary>
	/// Size of the page (25 is the max).
	/// </summary>
	public int? PageSize { get; set; }
	
	/// <summary>
	/// Search query.
	/// </summary>
	public string Query { get; set; }
	
	/// <summary>
	/// Return entries starting with the given letter.
	/// </summary>
	public char? Letter { get; set; }

	/// <summary>
	/// Select order by property.
	/// </summary>
	public CharacterSearchOrderBy OrderBy { get; set; }

	/// <summary>
	/// Define sort direction for <see cref="OrderBy">OrderBy</see> property.
	/// </summary>
	public SortDirection SortDirection { get; set; }

    ICollection<QueryParameter> ISearchConfig.GetQueryParameters()
    {
	    var parameters = new List<QueryParameter>
	    {
		    QueryParameter.Page(Page),
		    QueryParameter.Limit(PageSize),
		    QueryParameter.Query(Query),
		    QueryParameter.Letter(Letter)
	    };

	    if (OrderBy != CharacterSearchOrderBy.NoSorting)
	    {
		    parameters.Add(QueryParameter.OrderBy(OrderBy));
		    parameters.Add(QueryParameter.Sort(SortDirection));
	    }

	    return parameters;
    }
}