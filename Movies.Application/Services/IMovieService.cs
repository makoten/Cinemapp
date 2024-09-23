using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie);

    Task<Movie?> GetByIdAsync(Guid id, CancellationToken token, Guid? userId);

    Task<Movie?> GetBySlugAsync(string slug, CancellationToken token, Guid? userId);

    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token, Guid? userId);

    Task<Movie?> UpdateAsync(Movie movie, CancellationToken token, Guid? userId);

    Task<bool> DeleteByIdAsync(Guid id);
}