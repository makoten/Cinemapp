using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

internal class MovieService(
    IMovieRepository movieRepository,
    IValidator<Movie> movieValidator,
    IRatingRepository ratingRepository,
    IValidator<GetAllMoviesOptions> optionsValidator)
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

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token )
    {
        await optionsValidator.ValidateAndThrowAsync(options, token);
        return await movieRepository.GetAllAsync(options, token);
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
        await movieValidator.ValidateAndThrowAsync(movie, token);
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