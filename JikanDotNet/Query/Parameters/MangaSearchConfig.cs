using JikanDotNet.Interfaces;
using System.Collections.Generic;
using JikanDotNet.Query;

namespace JikanDotNet
{
	/// <summary>
	/// Model class of search configuration for advanced manga search.
	/// </summary>
	public class MangaSearchConfig : ISearchConfig
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
		/// Manga type of searched result.
		/// </summary>
		public MangaType Type { get; set; } = MangaType.EveryType;

		/// <summary>
		/// Minimum score results (1-10).
		/// </summary>
		public int? MinimumScore { get; set; }
		
		/// <summary>
		/// Maximum score results (1-10).
		/// </summary>
		public int? MaximumScore { get; set; }

		/// <summary>
		/// Current status.
		/// </summary>
		public PublishingStatus Status { get; set; }

		/// <summary>
		/// Select order property.
		/// </summary>
		public MangaSearchOrderBy OrderBy { get; set; }

		/// <summary>
		/// Define sort direction for <see cref="OrderBy">OrderBy</see> property.
		/// </summary>
		public SortDirection SortDirection { get; set; }

		/// <summary>
		/// Genres to include.
		/// </summary>
		public ICollection<MangaGenreSearch> Genres { get; set; } = new List<MangaGenreSearch>();
		
		/// <summary>
		/// Genres to exclude.
		/// </summary>
		public ICollection<MangaGenreSearch> ExcludedGenres { get; set; } = new List<MangaGenreSearch>();

		/// <summary>
		/// Filter by magazine id.
		/// </summary>
		public ICollection<long> MagazineIds { get; set; } = new List<long>();
		
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
				QueryParameter.EndDate(EndDate),
				QueryParameter.SafeForWork(Sfw),
				QueryParameter.Unapproved(Unapproved),
				QueryParameter.StartDate(StartDate),
				QueryParameter.MinScore(MinimumScore),
				QueryParameter.MaxScore(MaximumScore),
				QueryParameter.Magazines(MagazineIds),
				QueryParameter.Genres(Genres),
				QueryParameter.ExcludedGenres(ExcludedGenres)
			};

			if (Type != MangaType.EveryType)
			{
				parameters.Add(QueryParameter.Type(Type));
			}

			if (Status != PublishingStatus.EveryStatus)
			{
				parameters.Add(QueryParameter.Status(Status));
			}

			if (OrderBy != MangaSearchOrderBy.NoSorting)
			{
				parameters.Add(QueryParameter.OrderBy(OrderBy));
				parameters.Add(QueryParameter.Sort(SortDirection));
			}

			return parameters;
		}
	}
}