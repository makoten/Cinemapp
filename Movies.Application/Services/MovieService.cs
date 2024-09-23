using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

internal class MovieService(
    IMovieRepository movieRepository,
    IValidator<Movie> movieValidator,
    IRatingRepository ratingRepository)
    : IMovieService
{
    public async Task<bool> CreateAsync(Movie movie)
    {
        await movieValidator.ValidateAndThrowAsync(movie);
        return await movieRepository.CreateAsync(movie);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return movieRepository.DeleteByIdAsync(id);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token, Guid? userId = default)
    {
        return movieRepository.GetAllAsync(userId, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default, Guid? userId = default)
    {
        return movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default, Guid? userId = default)
    {
        return movieRepository.GetBySlugAsync(slug, token, userId);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token, Guid? userId = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var movieExists = await movieRepository.ExistsByIdAsync(movie.Id);
        if (!movieExists)
            return null;

        await movieRepository.UpdateAsync(movie, userId, token);

        if (!userId.HasValue)
        {
            var rating = await ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

        var ratings = await ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;

        return movie;
    }
}