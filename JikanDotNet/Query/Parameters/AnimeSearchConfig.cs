using JikanDotNet.Interfaces;
using System.Collections.Generic;
using JikanDotNet.Query;

namespace JikanDotNet
{
	/// <summary>
	/// Model class of search configuration for advanced anime search.
	/// </summary>
	public class AnimeSearchConfig : ISearchConfig
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
		/// Anime type of searched result.
		/// </summary>
		public AnimeType Type { get; set; } = AnimeType.EveryType;

		/// <summary>
		/// Minimum score results (1-10).
		/// </summary>
		public int? MinimumScore { get; set; }
		
		/// <summary>
		/// Maximum score results (1-10).
		/// </summary>
		public int? MaximumScore { get; set; }

		/// <summary>
		/// Age rating.
		/// </summary>
		public AnimeAgeRating Rating { get; set; } = AnimeAgeRating.EveryRating;

		/// <summary>
		/// Current status.
		/// </summary>
		public AiringStatus Status { get; set; }

		/// <summary>
		/// Select order property.
		/// </summary>
		public AnimeSearchOrderBy OrderBy { get; set; }

		/// <summary>
		/// Define sort direction for <see cref="OrderBy">OrderBy</see> property.
		/// </summary>
		public SortDirection SortDirection { get; set; }

		/// <summary>
		/// Genres to include.
		/// </summary>
		public ICollection<AnimeGenreSearch> Genres { get; set; } = new List<AnimeGenreSearch>();
		
		/// <summary>
		/// Genres to exclude.
		/// </summary>
		public ICollection<MangaGenreSearch> ExcludedGenres { get; set; } = new List<MangaGenreSearch>();

		/// <summary>
		/// Filter by producer id.
		/// </summary>
		public ICollection<long> ProducerIds { get; set; } = new List<long>();

		/// <summary>
		/// Should only search for sfw title. Filter out adult entries.
		/// </summary>
		public bool Sfw { get; set; } = true;

		/// <summary>
		/// Should include unapproved entries.
		/// </summary>
		public bool Unapproved { get; set; }

		/// <summary>
		/// The starting date.
		/// </summary>
		/// <remarks> The available formats are: <ul>
		/// <li><c>2022</c></li>
		/// <li><c>2022-05</c></li>
		/// <li><c>2022-05-23</c></li>
		/// </ul> </remarks>
		public string StartDate { get; set; }

		/// <summary>
		/// The ending date.
		/// </summary>
		/// <remarks> The available formats are: <ul>
		/// <li><c>2022</c></li>
		/// <li><c>2022-05</c></li>
		/// <li><c>2022-05-23</c></li>
		/// </ul> </remarks>
		public string EndDate { get; set; }

		ICollection<QueryParameter> ISearchConfig.GetQueryParameters()
		{
			var parameters = new List<QueryParameter>
			{
				QueryParameter.Page(Page),
				QueryParameter.Limit(PageSize),
				QueryParameter.Query(Query),
				QueryParameter.Letter(Letter),
				QueryParameter.SafeForWork(Sfw),
				QueryParameter.StartDate(StartDate),
				QueryParameter.EndDate(EndDate),
				QueryParameter.Unapproved(Unapproved),
				QueryParameter.Producers(ProducerIds),
				QueryParameter.MaxScore(MaximumScore),
				QueryParameter.MinScore(MinimumScore),
				QueryParameter.Genres(Genres),
				QueryParameter.ExcludedGenres(ExcludedGenres)
			};

			if (Type != AnimeType.EveryType)
			{
				parameters.Add(QueryParameter.Type(Type));
			}

			if (Rating != AnimeAgeRating.EveryRating)
			{
				parameters.Add(QueryParameter.Rating(Rating));
			}

			if (Status != AiringStatus.EveryStatus)
			{
				parameters.Add(QueryParameter.Status(Status));
			}

			if (OrderBy != AnimeSearchOrderBy.NoSorting)
			{
				parameters.Add(QueryParameter.OrderBy(OrderBy));
				parameters.Add(QueryParameter.Sort(SortDirection));
			}

			return parameters;
		}
	}
}