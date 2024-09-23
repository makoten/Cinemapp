using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

internal class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;
    private readonly IRatingRepository _ratingRepository;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator,
        IRatingRepository ratingRepository)
    {
        _movieValidator = movieValidator;
        _movieRepository = movieRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        return await _movieRepository.CreateAsync(movie);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return _movieRepository.DeleteByIdAsync(id);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token, Guid? userId = default)
    {
        return _movieRepository.GetAllAsync(userId, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default, Guid? userId = default)
    {
        return _movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default, Guid? userId = default)
    {
        return _movieRepository.GetBySlugAsync(slug, token, userId);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token, Guid? userId = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id);
        if (!movieExists)
            return null;

        await _movieRepository.UpdateAsync(movie, userId, token);

        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

        var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;

        return movie;
    }
}