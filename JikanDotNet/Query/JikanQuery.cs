using System.Collections.Generic;
using System.Text;

namespace JikanDotNet.Query;

internal class JikanQuery : IQuery
{
    private readonly string _endpoint;
    private readonly HashSet<QueryParameter> _parameters = [];

    internal JikanQuery(params string[] endpointParts)
    {
#if NETCOREAPP3_1_OR_GREATER
        _endpoint = string.Join('/', endpointParts);
#else
        _endpoint = string.Join("/", endpointParts);
#endif
    }

    internal JikanQuery WithParameter(string name, bool value = true)
    {
        if (value) _parameters.Add(new TextQueryParameter(name));
        return this;
    }

    internal JikanQuery WithParameter(string name, string value)
    {
        _parameters.Add(new KeyValueQueryParameter(name, value));
        return this;
    }

    public string GetQuery()
    {
        var sb = new StringBuilder(_endpoint);

        if (_parameters.Count > 0)
        {
            sb.Append('?').AppendJoin('&', _parameters);
        }

        return sb.ToString();
    }
}
