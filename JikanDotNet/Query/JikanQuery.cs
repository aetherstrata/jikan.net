using System.Collections;
using System.Collections.Generic;
using System.Text;
using JikanDotNet.Exceptions;
using JikanDotNet.Interfaces;

namespace JikanDotNet.Query;

internal class JikanQuery : IQuery, IEnumerable<QueryParameter>
{
	private readonly string _endpoint;
	private readonly HashSet<QueryParameter> _parameters = new(QueryParameter.NameComparer);

	internal JikanQuery(params string[] endpointParts)
	{
		_endpoint = string.Join("/", endpointParts);
	}

	internal JikanQuery Add(QueryParameter parameter)
	{
		if (parameter != null && !_parameters.Add(parameter))
		{
			throw new JikanDuplicateParameterException(parameter.Name, $"Parameter '{parameter.Name}' has been set already");
		}
		return this;
	}

	internal JikanQuery Add(ISearchConfig searchConfig)
	{
		if (searchConfig != null)
		{
			foreach (var parameter in searchConfig.GetQueryParameters())
			{
				Add(parameter);
			}
		}
		return this;
	}

	public string GetQuery()
	{
		var sb = new StringBuilder(_endpoint);

		if (_parameters.Count > 0)
		{
			sb.Append('?').Append(string.Join("&", _parameters));
		}

		return sb.ToString();
	}

	public IEnumerator<QueryParameter> GetEnumerator() => _parameters.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_parameters).GetEnumerator();
}
