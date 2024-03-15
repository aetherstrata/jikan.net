using FluentAssertions;
using JikanDotNet.Exceptions;
using Xunit;

using JikanDotNet.Query;

namespace JikanDotNet.Tests
{
    public class QueryTests
    {
        private static readonly string[] EndpointParts =
        [
            "api",
            "v2",
            "items"
        ];

        private static readonly string Endpoint = string.Join('/', EndpointParts);

        private readonly JikanQuery _query;

        public QueryTests()
        {
            _query = new JikanQuery(EndpointParts);
        }

        [Fact]
        public void GetQuery_shouldReturnEndpoint_whenNoParams()
        {
            _query.GetQuery().Should().Be(Endpoint);
        }

        [Fact]
        public void GetQuery_shouldReturnEndpoint_whenNullParameter()
        {
            QueryParameter param = null;

            _query.Add(param);

            _query.GetQuery().Should().Be(Endpoint);
        }

        [Fact]
        public void GetQuery_shouldAppendParameter()
        {
            var param = new QueryParameter("param1", "value1");

            _query.Add(param);

            _query.GetQuery().Should().Be(Endpoint + "?param1=value1");
        }

        [Fact]
        public void GetQuery_shouldConcatMultipleParameters()
        {
            var param1 = new QueryParameter("param1", "value1");
            var param2 = new QueryParameter("param2", "value2");

            _query.Add(param1).Add(param2);

            _query.GetQuery().Should().Be(Endpoint + "?param1=value1&param2=value2");
        }

        [Fact]
        public void Add_shouldThrow_whenDuplicateParameter()
        {
            var param = new QueryParameter("param1", "value1");

            _query.Add(param);

            _query.Invoking(q => q.Add(param)).Should().ThrowExactly<JikanDuplicateParameterException>();
        }
    }
}