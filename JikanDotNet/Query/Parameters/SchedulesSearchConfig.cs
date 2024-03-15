using System.Collections.Generic;
using JikanDotNet.Interfaces;
using JikanDotNet.Query;

namespace JikanDotNet
{
    /// <summary>
    /// Model class of search configuration for advanced shedule search.
    /// </summary>
    public class SchedulesSearchConfig : ISearchConfig
    {
        /// <summary>
        /// Index of page folding 25 records of top ranging (e.g. 1 will return first 25 records, 2 will return record from 26 to 50 etc.)
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Size of the page (25 is the max).
        /// </summary>
        public int? PageSize { get; set; }

        public ScheduledDay Filter { get; set; }

        /// <summary>
        /// Should only search for sfw titles. Filter entries with the Hentai genre.
        /// </summary>
        public bool Sfw { get; set; }

        /// <summary>
        /// Should include unapproved entries.
        /// </summary>
        public bool Unapproved { get; set; }

        /// <summary>
        /// Should filter Kid entries.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, it will return only Kid entries and when <c>false</c>, it will filter out any Kid entries.
        /// </remarks>
        public bool? Kids { get; set; }

        ICollection<QueryParameter> ISearchConfig.GetQueryParameters()
        {
            var parameters = new List<QueryParameter>()
            {
                QueryParameter.Page(Page),
                QueryParameter.Limit(PageSize),
                QueryParameter.Filter(Filter),
                QueryParameter.SafeForWork(Sfw),
                QueryParameter.Unapproved(Unapproved),
            };

            if (Kids.HasValue)
            {
                parameters.Add(QueryParameter.Kids(Kids.Value));
            }

            return parameters;
        }
    }
}
