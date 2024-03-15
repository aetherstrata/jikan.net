using System.Collections.Generic;
using JikanDotNet.Query;

namespace JikanDotNet.Interfaces
{
	/// <summary>
	/// Interface of search config for advanced searching.
	/// </summary>
	internal interface ISearchConfig
	{
		/// <summary>
		/// Build query parameter set from search config
		/// </summary>
		/// <returns>Current parameters for search request</returns>
		ICollection<QueryParameter> GetQueryParameters();
	}
}