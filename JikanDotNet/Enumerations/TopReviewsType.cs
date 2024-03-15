using System.ComponentModel;

namespace JikanDotNet
{
    /// <summary>
    /// The type of reviews to filter by.
    /// </summary>
    public enum TopReviewsType
    {
        /// <summary>
        /// Anime.
        /// </summary>
        [Description("anime")]
        Anime,

        /// <summary>
        /// Manga.
        /// </summary>
        [Description("manga")]
        Manga
    }
}
