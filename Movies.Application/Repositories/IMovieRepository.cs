using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie);

    Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token);

    Task<Movie?> GetBySlugAsync(string slug, CancellationToken token, Guid? userId = default);

    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token);

    Task<bool> DeleteByIdAsync(Guid id);

    Task<bool> UpdateAsync(Movie movie, Guid? userId, CancellationToken token);

    Task<bool> ExistsByIdAsync(Guid id);

    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token);
}