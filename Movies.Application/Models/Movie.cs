
using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }

    public required List<string> Genres { get; init; } = new();

    public float? Rating { get; set; }

    public int? UserRating { get; set; }

    public string Slug => GenerateSlug();

    private string GenerateSlug()
    {
        var sluggedTitle = Regex
                .Replace(Title, "[^0-9A-Za-z _-]", string.Empty)
                .ToLower()
                .Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }


}
