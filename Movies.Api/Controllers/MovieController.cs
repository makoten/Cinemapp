using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;


[ApiController]
public class MovieController : ControllerBase
{
    private IMovieRepository _movieRepository;

    public MovieController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    // [HttpPost("movies")] is really poorly hardcoded, so I leverage a static function
    // to centralize the URI declarations
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        var createdMovie = await _movieRepository.CreateAsync(movie);
        var response = new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres
        };

        return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", response);
    }
}
