using FluentValidation.Results;
using Movies.Application.Repositories;
using ValidationException = FluentValidation.ValidationException;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            var failure = new ValidationFailure("Rating", "Rating must be between 1 and 5");
            throw new ValidationException([failure]);
        }

        var movieExists = await movieRepository.ExistsByIdAsync(movieId);
        return movieExists && await ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token)
    {
        var movieExists = await movieRepository.ExistsByIdAsync(movieId);
        return movieExists && await ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }
}