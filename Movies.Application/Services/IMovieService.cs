using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie);

    Task<Movie?> GetByIdAsync(Guid id, CancellationToken token, Guid? userId);

    Task<Movie?> GetBySlugAsync(string slug, CancellationToken token, Guid? userId);

    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token);

    Task<Movie?> UpdateAsync(Movie movie, CancellationToken token, Guid? userId);

    Task<bool> DeleteByIdAsync(Guid id);

    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token);
}