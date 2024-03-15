using System.Collections.Generic;
using JikanDotNet.Interfaces;
using JikanDotNet.Query;

namespace JikanDotNet;

/// <summary>
/// Model class of search configuration for advanced user search.
/// </summary>
public class UserSearchConfig: ISearchConfig
{
    /// <summary>
	/// Index of page folding 50 records of top ranging (e.g. 1 will return first 50 records, 2 will return record from 51 to 100 etc.)
	/// </summary>
	public int? Page { get; set; }
    
    /// <summary>
	/// Search query.
	/// </summary>
	public string Query { get; set; }
	
	/// <summary>
	/// Gender of the user.
	/// </summary>
	public UserGender Gender { get; set; }
	
	/// <summary>
	/// Location of the searched users.
	/// </summary>
	public string Location { get; set; }
	
	/// <summary>
	/// Max age of the searched users.
	/// </summary>
	public int? MaxAge { get; set; }
	
	/// <summary>
	/// Min age of the searched users.
	/// </summary>
	public int? MinAge { get; set; }

    ICollection<QueryParameter> ISearchConfig.GetQueryParameters()
    {
	    var parameters = new List<QueryParameter>
	    {
		    QueryParameter.Page(Page),
		    QueryParameter.MinAge(MinAge),
		    QueryParameter.MaxAge(MaxAge),
		    QueryParameter.Query(Query),
		    QueryParameter.Location(Location)
	    };

	    if (Gender != UserGender.Any)
	    {
		    parameters.Add(QueryParameter.Gender(Gender));
	    }

	    return parameters;
    }
}