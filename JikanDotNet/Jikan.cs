using JikanDotNet.Config;
using JikanDotNet.Consts;
using JikanDotNet.Exceptions;
using JikanDotNet.Extensions;
using JikanDotNet.Helpers;
using JikanDotNet.Limiter;
using JikanDotNet.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JikanDotNet
{
	/// <summary>
	/// Implementation of Jikan wrapper for .Net platform.
	/// </summary>
	public class Jikan : IJikan
	{
		#region Fields

		/// <summary>
		/// Http client class to call REST request and receive REST response.
		/// </summary>
		private readonly HttpClient _httpClient;

		/// <summary>
		/// Client configuration.
		/// </summary>
		private readonly JikanClientConfiguration _jikanConfiguration;

		/// <summary>
		/// API call limiter
		/// </summary>
		private readonly ITaskLimiter _limiter;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Jikan() : this(new JikanClientConfiguration())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="jikanClientConfiguration">Options.</param>
		/// <param name="httpClient">Http client.</param>
		public Jikan(JikanClientConfiguration jikanClientConfiguration, HttpClient httpClient = null)
		{
			_jikanConfiguration = jikanClientConfiguration;
			_limiter = new CompositeTaskLimiter(jikanClientConfiguration.LimiterConfigurations?.Distinct() ?? TaskLimiterConfiguration.None);
			_httpClient = httpClient ?? DefaultHttpClientProvider.GetDefaultHttpClient();
		}

		#endregion Constructors

		#region Private Methods

		/// <summary>
		/// Basic method for handling requests and responses from endpoint.
		/// </summary>
		/// <typeparam name="T">Class type received from GET requests.</typeparam>
		/// <param name="query">Endpoint query.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Requested object if successful, null otherwise.</returns>
		private async Task<T> ExecuteGetRequestAsync<T>(IQuery query, CancellationToken ct = default)
			where T : class
		{
			try
			{
				using var response = await _limiter.LimitAsync(() => _httpClient.GetAsync(query.GetQuery(), ct));
				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsByteArrayAsync();
					return JsonSerializer.Deserialize<T>(json);
				}

				if (!_jikanConfiguration.SuppressException)
				{
					var json = await response.Content.ReadAsByteArrayAsync();
					var errorData = JsonSerializer.Deserialize<JikanApiError>(json);
					throw new JikanRequestException(
						string.Format(ErrorMessagesConst.FailedRequest, response.StatusCode, response.Content),
						errorData);
				}
			}
			catch (JsonException ex)
			{
				if (!_jikanConfiguration.SuppressException)
				{
					throw new JikanRequestException(
						ErrorMessagesConst.SerializationFailed + Environment.NewLine + "Inner exception message: " + ex.Message,
						ex);
				}
			}
			return null;
		}

		#endregion Private Methods

		#region Public Methods

		#region Anime methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<Anime>> GetAnimeAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<Anime>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<AnimeCharacter>>> GetAnimeCharactersAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Characters);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<AnimeCharacter>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<AnimeStaffPosition>>> GetAnimeStaffAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Staff);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<AnimeStaffPosition>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<AnimeEpisode>>> GetAnimeEpisodesAsync(long id, int page, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Episodes)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<AnimeEpisode>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<AnimeEpisode>>> GetAnimeEpisodesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Episodes);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<AnimeEpisode>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<AnimeEpisode>> GetAnimeEpisodeAsync(long animeId, int episodeId, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(animeId, nameof(animeId));
			Guard.IsGreaterThanZero(episodeId, nameof(episodeId));
			var query = new JikanQuery(JikanEndpointConsts.Anime, animeId.ToString(), JikanEndpointConsts.Episodes, episodeId.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<AnimeEpisode>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<News>>> GetAnimeNewsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.News);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<News>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<News>>> GetAnimeNewsAsync(long id, int page, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.News)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<News>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ForumTopic>>> GetAnimeForumTopicsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Forum);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ForumTopic>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ForumTopic>>> GetAnimeForumTopicsAsync(long id,
			ForumTopicType type, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Forum)
			{
				QueryParameter.Filter(type)
			};
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ForumTopic>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<AnimeVideos>> GetAnimeVideosAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Videos);
			return await ExecuteGetRequestAsync<BaseJikanResponse<AnimeVideos>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ImagesSet>>> GetAnimePicturesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Pictures);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ImagesSet>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<AnimeStatistics>> GetAnimeStatisticsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Statistics);
			return await ExecuteGetRequestAsync<BaseJikanResponse<AnimeStatistics>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<MoreInfo>> GetAnimeMoreInfoAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.MoreInfo);
			return await ExecuteGetRequestAsync<BaseJikanResponse<MoreInfo>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<Recommendation>>> GetAnimeRecommendationsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Recommendations);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<Recommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<AnimeUserUpdate>>> GetAnimeUserUpdatesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.UserUpdates);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<AnimeUserUpdate>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<AnimeUserUpdate>>> GetAnimeUserUpdatesAsync(long id,
			int page, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.UserUpdates)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<AnimeUserUpdate>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetAnimeReviewsAsync(long id,
			bool includePreliminary = true, bool includeSpoiler = false, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Reviews)
			{
				QueryParameter.Preliminary(includePreliminary),
				QueryParameter.Spoiler(includeSpoiler)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetAnimeReviewsAsync(long id, int page,
			bool includePreliminary = true, bool includeSpoiler = false, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Reviews)
			{
				QueryParameter.Page(page),
				QueryParameter.Preliminary(includePreliminary),
				QueryParameter.Spoiler(includeSpoiler)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<RelatedEntry>>> GetAnimeRelationsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Relations);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<RelatedEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<AnimeThemes>> GetAnimeThemesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Themes);
			return await ExecuteGetRequestAsync<BaseJikanResponse<AnimeThemes>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ExternalLink>>> GetAnimeExternalLinksAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.External);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ExternalLink>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ExternalLink>>> GetAnimeStreamingLinksAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Streaming);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ExternalLink>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<AnimeFull>> GetAnimeFullDataAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Anime, id.ToString(), JikanEndpointConsts.Full);
			return await ExecuteGetRequestAsync<BaseJikanResponse<AnimeFull>>(query, ct);
		}

		#endregion Anime methods

		#region Character methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<Character>> GetCharacterAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Characters, id.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<Character>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<CharacterAnimeographyEntry>>> GetCharacterAnimeAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Characters, id.ToString(), JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<CharacterAnimeographyEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<CharacterMangaographyEntry>>> GetCharacterMangaAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Characters, id.ToString(), JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<CharacterMangaographyEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<VoiceActorEntry>>> GetCharacterVoiceActorsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Characters, id.ToString(), JikanEndpointConsts.Voices);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<VoiceActorEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ImagesSet>>> GetCharacterPicturesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Characters, id.ToString(), JikanEndpointConsts.Pictures);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ImagesSet>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<CharacterFull>> GetCharacterFullDataAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Characters, id.ToString(), JikanEndpointConsts.Full);
			return await ExecuteGetRequestAsync<BaseJikanResponse<CharacterFull>>(query, ct);
		}

		#endregion Character methods

		#region Manga methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<Manga>> GetMangaAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<Manga>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<MangaCharacter>>> GetMangaCharactersAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Characters);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<MangaCharacter>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<News>>> GetMangaNewsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.News);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<News>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<News>>> GetMangaNewsAsync(long id, int page, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.News)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<News>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ForumTopic>>> GetMangaForumTopicsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Forum);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ForumTopic>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ImagesSet>>> GetMangaPicturesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Pictures);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ImagesSet>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<MangaStatistics>> GetMangaStatisticsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Statistics);
			return await ExecuteGetRequestAsync<BaseJikanResponse<MangaStatistics>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<MoreInfo>> GetMangaMoreInfoAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.MoreInfo);
			return await ExecuteGetRequestAsync<BaseJikanResponse<MoreInfo>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<MangaUserUpdate>>> GetMangaUserUpdatesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.UserUpdates);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<MangaUserUpdate>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<MangaUserUpdate>>> GetMangaUserUpdatesAsync(long id, int page, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.UserUpdates)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<MangaUserUpdate>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<Recommendation>>> GetMangaRecommendationsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Recommendations);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<Recommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetMangaReviewsAsync(long id,
			bool includePreliminary = true, bool includeSpoiler = false, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Reviews)
			{
				QueryParameter.Preliminary(includePreliminary),
				QueryParameter.Spoiler(includeSpoiler)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetMangaReviewsAsync(long id, int page,
			bool includePreliminary = true, bool includeSpoiler = false, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Reviews)
			{
				QueryParameter.Page(page),
				QueryParameter.Preliminary(includePreliminary),
				QueryParameter.Spoiler(includeSpoiler)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<RelatedEntry>>> GetMangaRelationsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Relations);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<RelatedEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ExternalLink>>> GetMangaExternalLinksAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.External);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ExternalLink>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<MangaFull>> GetMangaFullDataAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Manga, id.ToString(), JikanEndpointConsts.Full);
			return await ExecuteGetRequestAsync<BaseJikanResponse<MangaFull>>(query, ct);
		}

		#endregion Manga methods

		#region Person methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<Person>> GetPersonAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.People, id.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<Person>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<PersonAnimeographyEntry>>> GetPersonAnimeAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.People, id.ToString(), JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<PersonAnimeographyEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<PersonMangaographyEntry>>> GetPersonMangaAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.People, id.ToString(), JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<PersonMangaographyEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<VoiceActingRole>>> GetPersonVoiceActingRolesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.People, id.ToString(), JikanEndpointConsts.Voices);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<VoiceActingRole>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ImagesSet>>> GetPersonPicturesAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.People, id.ToString(), JikanEndpointConsts.Pictures);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ImagesSet>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<PersonFull>> GetPersonFullDataAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.People, id.ToString(), JikanEndpointConsts.Full);
			return await ExecuteGetRequestAsync<BaseJikanResponse<PersonFull>>(query, ct);
		}

		#endregion Person methods

		#region Season methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetSeasonAsync(int year, Season season, CancellationToken ct = default)
		{
			Guard.IsValid(x => x is >= 1000 and < 10000, year, nameof(year));
			Guard.IsValidEnum(season, nameof(season));
			var query = new JikanQuery(JikanEndpointConsts.Seasons, year.ToString(), season.GetDescription());
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetSeasonAsync(int year, Season season, int page, CancellationToken ct = default)
		{
			Guard.IsValid(x => x is >= 1000 and < 10000, year, nameof(year));
			Guard.IsValidEnum(season, nameof(season));
			var query = new JikanQuery(JikanEndpointConsts.Seasons, year.ToString(), season.GetDescription())
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<SeasonArchive>>> GetSeasonArchiveAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Seasons);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<SeasonArchive>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetCurrentSeasonAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Seasons, JikanEndpointConsts.Now);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetCurrentSeasonAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Seasons, JikanEndpointConsts.Now)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetUpcomingSeasonAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Seasons, JikanEndpointConsts.Upcoming);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetUpcomingSeasonAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Seasons, JikanEndpointConsts.Upcoming)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		#endregion Season methods

		#region Schedule methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetScheduleAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Schedules);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetScheduleAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Schedules)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetScheduleAsync(ScheduledDay scheduledDay, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Schedules)
			{
				QueryParameter.Filter(scheduledDay)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetScheduleAsync(SchedulesSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Schedules)
			{
				searchConfig
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		#endregion Schedule methods

		#region Top methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetTopAnimeAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetTopAnimeAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Anime)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetTopAnimeAsync(TopAnimeFilter filter, CancellationToken ct = default)
		{
			Guard.IsValidEnum(filter, nameof(filter));
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Anime)
			{
				QueryParameter.Filter(filter)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> GetTopAnimeAsync(TopAnimeFilter filter, int page, CancellationToken ct = default)
		{
			Guard.IsValidEnum(filter, nameof(filter));
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Anime)
			{
				QueryParameter.Page(page),
				QueryParameter.Filter(filter)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Manga>>> GetTopMangaAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Manga>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Manga>>> GetTopMangaAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Manga)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Manga>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Person>>> GetTopPeopleAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.People);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Person>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Person>>> GetTopPeopleAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.People)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Person>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Character>>> GetTopCharactersAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Characters);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Character>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Character>>> GetTopCharactersAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Characters)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Character>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetTopReviewsAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Reviews);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetTopReviewsAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Reviews)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetTopReviewsAsync(TopReviewsType type, int page,
			bool includePreliminary = true, bool includeSpoilers = true, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.TopList, JikanEndpointConsts.Reviews)
			{
				QueryParameter.Page(page),
				QueryParameter.Type(type),
				QueryParameter.Preliminary(includePreliminary),
				QueryParameter.Spoilers(includeSpoilers)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		#endregion Top methods

		#region Genre methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<Genre>>> GetAnimeGenresAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Genres, JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<Genre>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<Genre>>> GetAnimeGenresAsync(GenresFilter filter, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Genres, JikanEndpointConsts.Anime)
			{
				QueryParameter.Filter(filter)
			};
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<Genre>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<Genre>>> GetMangaGenresAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Genres, JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<Genre>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<Genre>>> GetMangaGenresAsync(GenresFilter filter, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Genres, JikanEndpointConsts.Manga)
			{
				QueryParameter.Filter(filter)
			};
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<Genre>>>(query, ct);
		}

		#endregion Genre methods

		#region Producer methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Producer>>> GetProducersAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Producers);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Producer>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Producer>>> GetProducersAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Producers)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Producer>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<Producer>> GetProducerAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Producers, id.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<Producer>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ExternalLink>>> GetProducerExternalLinksAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Producers, id.ToString(), JikanEndpointConsts.External);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ExternalLink>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ProducerFull>> GetProducerFullDataAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Producers, id.ToString(), JikanEndpointConsts.Full);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ProducerFull>>(query, ct);
		}

		#endregion Producer methods

		#region Magazine methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Magazine>>> GetMagazinesAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Magazines);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Magazine>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Magazine>>> GetMagazinesAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Magazines)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Magazine>>>(query, ct);
		}

		#endregion Magazine methods

		#region Club methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<Club>> GetClubAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Clubs, id.ToString());
			return await ExecuteGetRequestAsync<BaseJikanResponse<Club>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<ClubMember>>> GetClubMembersAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Clubs, id.ToString(), JikanEndpointConsts.Members);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<ClubMember>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<ClubMember>>> GetClubMembersAsync(long id, int page, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Clubs, id.ToString(), JikanEndpointConsts.Members)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<ClubMember>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ClubStaff>>> GetClubStaffAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Clubs, id.ToString(), JikanEndpointConsts.Staff);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ClubStaff>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ClubRelations>> GetClubRelationsAsync(long id, CancellationToken ct = default)
		{
			Guard.IsGreaterThanZero(id, nameof(id));
			var query = new JikanQuery(JikanEndpointConsts.Clubs, id.ToString(), JikanEndpointConsts.Relations);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ClubRelations>>(query, ct);
		}

		#endregion Club methods

		#region User methods

		/// <inheritdoc />
		public async Task<BaseJikanResponse<UserProfile>> GetUserProfileAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserProfile>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<UserStatistics>> GetUserStatisticsAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Statistics);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserStatistics>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<UserFavorites>> GetUserFavoritesAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Favorites);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserFavorites>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<UserUpdates>> GetUserUpdatesAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.UserUpdates);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserUpdates>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<UserAbout>> GetUserAboutAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.About);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserAbout>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<HistoryEntry>>> GetUserHistoryAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.History);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<HistoryEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<HistoryEntry>>> GetUserHistoryAsync(string username,
			UserHistoryExtension filter, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.History)
			{
				QueryParameter.Filter(filter)
			};
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<HistoryEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<AnimeListEntry>>> GetUserAnimeListAsync(string username, CancellationToken ct = default)
		{
			Guard.IsDefaultEndpoint(_httpClient.BaseAddress.ToString(), nameof(GetUserAnimeListAsync));
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.AnimeList);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<AnimeListEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<AnimeListEntry>>> GetUserAnimeListAsync(string username,
			int page, CancellationToken ct = default)
		{
			Guard.IsDefaultEndpoint(_httpClient.BaseAddress.ToString(), nameof(GetUserAnimeListAsync));
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.AnimeList)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<AnimeListEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<MangaListEntry>>> GetUserMangaListAsync(string username, CancellationToken ct = default)
		{
			Guard.IsDefaultEndpoint(_httpClient.BaseAddress.ToString(), nameof(GetUserMangaListAsync));
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.MangaList);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<MangaListEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<MangaListEntry>>> GetUserMangaListAsync(string username,
			int page, CancellationToken ct = default)
		{
			Guard.IsDefaultEndpoint(_httpClient.BaseAddress.ToString(), nameof(GetUserMangaListAsync));
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.MangaList)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<MangaListEntry>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Friend>>> GetUserFriendsAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Friends);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Friend>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Friend>>> GetUserFriendsAsync(string username, int page, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Friends)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Friend>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetUserReviewsAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Reviews);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetUserReviewsAsync(string username, int page, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Reviews)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserRecommendation>>> GetUserRecommendationsAsync(
			string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Recommendations);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserRecommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserRecommendation>>> GetUserRecommendationsAsync(
			string username, int page, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Recommendations)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserRecommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<MalUrl>>> GetUserClubsAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Clubs);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<MalUrl>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<MalUrl>>> GetUserClubsAsync(string username, int page, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Clubs)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<MalUrl>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<ICollection<ExternalLink>>> GetUserExternalLinksAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.External);
			return await ExecuteGetRequestAsync<BaseJikanResponse<ICollection<ExternalLink>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<BaseJikanResponse<UserFull>> GetUserFullDataAsync(string username, CancellationToken ct = default)
		{
			Guard.IsNotNullOrWhiteSpace(username, nameof(username));
			var query = new JikanQuery(JikanEndpointConsts.Users, username, JikanEndpointConsts.Full);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserFull>>(query, ct);
		}

		#endregion User methods

		#region GetRandom methods

		/// <inheritdoc/>
		public async Task<BaseJikanResponse<Anime>> GetRandomAnimeAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Random, JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<BaseJikanResponse<Anime>>(query, ct);
		}

		/// <inheritdoc/>
		public async Task<BaseJikanResponse<Manga>> GetRandomMangaAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Random, JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<BaseJikanResponse<Manga>>(query, ct);
		}

		/// <inheritdoc/>
		public async Task<BaseJikanResponse<Character>> GetRandomCharacterAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Random, JikanEndpointConsts.Characters);
			return await ExecuteGetRequestAsync<BaseJikanResponse<Character>>(query, ct);
		}

		/// <inheritdoc/>
		public async Task<BaseJikanResponse<Person>> GetRandomPersonAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Random, JikanEndpointConsts.People);
			return await ExecuteGetRequestAsync<BaseJikanResponse<Person>>(query, ct);
		}

		/// <inheritdoc/>
		public async Task<BaseJikanResponse<UserProfile>> GetRandomUserAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Random, JikanEndpointConsts.Users);
			return await ExecuteGetRequestAsync<BaseJikanResponse<UserProfile>>(query, ct);
		}

		#endregion GetRandom methods

		#region Watch methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<WatchEpisode>>> GetWatchRecentEpisodesAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Watch, JikanEndpointConsts.Episodes);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<WatchEpisode>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<WatchEpisode>>> GetWatchPopularEpisodesAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Watch, JikanEndpointConsts.Episodes, JikanEndpointConsts.Popular);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<WatchEpisode>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<WatchPromoVideo>>> GetWatchRecentPromosAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Watch, JikanEndpointConsts.Promos);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<WatchPromoVideo>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<WatchPromoVideo>>> GetWatchPopularPromosAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Watch, JikanEndpointConsts.Promos, JikanEndpointConsts.Popular);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<WatchPromoVideo>>>(query, ct);
		}

		#endregion

		#region Reviews methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetRecentAnimeReviewsAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Reviews, JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetRecentAnimeReviewsAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Reviews, JikanEndpointConsts.Anime)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetRecentMangaReviewsAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Reviews, JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Review>>> GetRecentMangaReviewsAsync(int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Reviews, JikanEndpointConsts.Manga)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Review>>>(query, ct);
		}

		#endregion

		#region Recommendations methods

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserRecommendation>>> GetRecentAnimeRecommendationsAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Recommendations, JikanEndpointConsts.Anime);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserRecommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserRecommendation>>> GetRecentAnimeRecommendationsAsync(
			int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Recommendations, JikanEndpointConsts.Anime)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserRecommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserRecommendation>>> GetRecentMangaRecommendationsAsync(CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Recommendations, JikanEndpointConsts.Manga);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserRecommendation>>>(query, ct);
		}

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserRecommendation>>> GetRecentMangaRecommendationsAsync(
			int page, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Recommendations, JikanEndpointConsts.Manga)
			{
				QueryParameter.Page(page)
			};
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserRecommendation>>>(query, ct);
		}

		#endregion

		#region Search methods

		/// <inheritdoc />
		public Task<PaginatedJikanResponse<ICollection<Anime>>> SearchAnimeAsync(string query, CancellationToken ct = default)
			=> SearchAnimeAsync(new AnimeSearchConfig { Query = query }, ct);

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Anime>>> SearchAnimeAsync(AnimeSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Anime).Add(searchConfig);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Anime>>>(query, ct);
		}

		/// <inheritdoc />
		public Task<PaginatedJikanResponse<ICollection<Manga>>> SearchMangaAsync(string query, CancellationToken ct = default)
			=> SearchMangaAsync(new MangaSearchConfig { Query = query }, ct);

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Manga>>> SearchMangaAsync(MangaSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Manga).Add(searchConfig);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Manga>>>(query, ct);
		}

		/// <inheritdoc />
		public Task<PaginatedJikanResponse<ICollection<Person>>> SearchPersonAsync(string query, CancellationToken ct = default)
			=> SearchPersonAsync(new PersonSearchConfig { Query = query }, ct);

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Person>>> SearchPersonAsync(
			PersonSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.People).Add(searchConfig);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Person>>>(query, ct);
		}

		/// <inheritdoc />
		public Task<PaginatedJikanResponse<ICollection<Character>>> SearchCharacterAsync(string query, CancellationToken ct = default)
			=> SearchCharacterAsync(new CharacterSearchConfig { Query = query }, ct);

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Character>>> SearchCharacterAsync(
			CharacterSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Characters).Add(searchConfig);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Character>>>(query, ct);
		}

		/// <inheritdoc />
		public Task<PaginatedJikanResponse<ICollection<UserMetadata>>> SearchUserAsync(string query, CancellationToken ct = default)
			=> SearchUserAsync(new UserSearchConfig { Query = query }, ct);

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<UserMetadata>>> SearchUserAsync(
			UserSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Users).Add(searchConfig);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<UserMetadata>>>(query, ct);
		}

		/// <inheritdoc />
		public Task<PaginatedJikanResponse<ICollection<Club>>> SearchClubAsync(string query, CancellationToken ct = default)
			=> SearchClubAsync(new ClubSearchConfig { Query = query }, ct);

		/// <inheritdoc />
		public async Task<PaginatedJikanResponse<ICollection<Club>>> SearchClubAsync(ClubSearchConfig searchConfig, CancellationToken ct = default)
		{
			var query = new JikanQuery(JikanEndpointConsts.Clubs).Add(searchConfig);
			return await ExecuteGetRequestAsync<PaginatedJikanResponse<ICollection<Club>>>(query, ct);
		}

		#endregion Search methods

		#endregion Public Methods
	}
}