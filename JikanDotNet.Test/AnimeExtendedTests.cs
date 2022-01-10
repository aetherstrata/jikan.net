﻿using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JikanDotNet.Tests
{
	public class AnimeExtendedTests
	{
		private readonly IJikan _jikan;

		public AnimeExtendedTests()
		{
			_jikan = new Jikan();
		}

		[Theory]
		[InlineData(long.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public async Task GetAnimeReviews_InvalidId_ShouldThrowValidationException(long malId)
		{
			// When
			Func<Task<AnimeReviews>> func = _jikan.Awaiting(x => x.GetAnimeReviews(malId));

			// Then
			await func.Should().ThrowExactlyAsync<JikanValidationException>();
		}

		[Fact]
		public async Task GetAnimeReviews_BebopId_ShouldParseCowboyBebopReviews()
		{
			// When
			var bebop = await _jikan.GetAnimeReviews(1);

			// Then
			var firstReview = bebop.Reviews.First();
			using (new AssertionScope())
			{
				firstReview.Reviewer.Username.Should().Be("TheLlama");
				firstReview.MalId.Should().Be(7406);
				firstReview.Reviewer.EpisodesSeen.Should().Be(26);
				firstReview.HelpfulCount.Should().BeGreaterThan(1400);

				firstReview.Reviewer.Scores.Overall.Should().Be(10);
				firstReview.Reviewer.Scores.Animation.Should().Be(9);
			}
		}

		[Theory]
		[InlineData(long.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public async Task GetAnimeReviews_SecondPageWithInvalidId_ShouldThrowValidationException(long malId)
		{
			// When
			Func<Task<AnimeReviews>> func = _jikan.Awaiting(x => x.GetAnimeReviews(malId, 2));

			// Then
			await func.Should().ThrowExactlyAsync<JikanValidationException>();
		}

		[Theory]
		[InlineData(int.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public async Task GetAnimeReviews_CorrectIdWrongPage_ShouldThrowValidationException(int page)
		{
			// When
			Func<Task<AnimeReviews>> func = _jikan.Awaiting(x => x.GetAnimeReviews(1, page));

			// Then
			await func.Should().ThrowExactlyAsync<JikanValidationException>();
		}

		[Fact]
		public async Task GetAnimeReviews_BebopIdSecondPage_ShouldParseCowboyBebopReviewsPaged()
		{
			// When
			var bebop = await _jikan.GetAnimeReviews(1, 2);

			// Then
			var firstReview = bebop.Reviews.First();
			using (new AssertionScope())
			{
				firstReview.Reviewer.EpisodesSeen.Should().Be(26);
				firstReview.HelpfulCount.Should().BeGreaterThan(5);
			}
		}

		[Theory]
		[InlineData(long.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public async Task GetAnimeUserUpdates_InvalidId_ShouldThrowValidationException(long malId)
		{
			// When
			Func<Task<AnimeUserUpdates>> func = _jikan.Awaiting(x => x.GetAnimeUserUpdates(malId));

			// Then
			await func.Should().ThrowExactlyAsync<JikanValidationException>();
		}

		[Fact]
		public async Task GetAnimeUserUpdates_BebopId_ShouldParseCowboyBebopUserUpdates()
		{
			// When
			var bebop = await _jikan.GetAnimeUserUpdates(1);

			// Then
			var firstUpdate = bebop.Updates.First();
			using (new AssertionScope())
			{
				bebop.Updates.Should().HaveCount(75);
				firstUpdate.Date.Value.Should().BeBefore(DateTime.Now);
			}
		}

		[Theory]
		[InlineData(long.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public async Task GetAnimeUserUpdates_SecondPageInvalidId_ShouldThrowValidationException(long malId)
		{
			// When
			Func<Task<AnimeUserUpdates>> func = _jikan.Awaiting(x => x.GetAnimeUserUpdates(malId, 2));

			// Then
			await func.Should().ThrowExactlyAsync<JikanValidationException>();
		}

		[Theory]
		[InlineData(int.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public async Task GetAnimeUserUpdates_ValidIdInvalidpage_ShouldThrowValidationException(int page)
		{
			// When
			Func<Task<AnimeUserUpdates>> func = _jikan.Awaiting(x => x.GetAnimeUserUpdates(1, page));

			// Then
			await func.Should().ThrowExactlyAsync<JikanValidationException>();
		}

		[Fact]
		public async Task GetAnimeUserUpdates_BebopIdSecondPage_ShouldParseCowboyBebopUserUpdatesPaged()
		{
			// When
			var bebop = await _jikan.GetAnimeUserUpdates(1, 2);

			// Then
			var firstUpdate = bebop.Updates.First();
			using (new AssertionScope())
			{
				bebop.Updates.Should().HaveCount(75);
				firstUpdate.EpisodesTotal.Should().HaveValue().And.Be(26);
			}
		}
	}
}