using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize("TrustedMember")]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie);
        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();


        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, token, userId)
            : await _movieService.GetBySlugAsync(idOrSlug, token, userId);

        if (movie is null)
            return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movies = await _movieService.GetAllAsync(token, userId);
        var moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [Authorize("TrustedMember")]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        movie = await _movieService.UpdateAsync(movie, token, userId);

        if (movie is null)
            return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize("Admin")]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var isDeleted = await _movieService.DeleteByIdAsync(id);
        if (!isDeleted)
            return NotFound();

        return NoContent();
    }
}