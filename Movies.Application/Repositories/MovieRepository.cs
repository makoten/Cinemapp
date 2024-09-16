using Movies.Application.Models;

namespace Movies.Application.Repositories;


public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = new();

    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        var movie = _movies.Find(m => m.Id == id);
        if (movie == null)
            return Task.FromResult(false);

        _movies.Remove(movie);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        var movies = _movies.AsEnumerable();
        return Task.FromResult(movies);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.SingleOrDefault(x => x.Id == id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        var movie = _movies.SingleOrDefault(x => x.Slug.Equals(slug));
        return Task.FromResult(movie);
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        if (movieIndex == -1)
        {
            return Task.FromResult(false);
        }

        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }
}
