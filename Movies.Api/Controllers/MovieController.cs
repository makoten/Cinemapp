using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;


[Authorize]
[ApiController]
public class MovieController : ControllerBase
{
    //private IMovieRepository _movieService;
    private IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie);
        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute]string idOrSlug, CancellationToken token)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, token)
            : await _movieService.GetBySlugAsync(idOrSlug, token);

        if (movie is null)
            return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var movies = await _movieService.GetAllAsync(token);
        var moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);
        movie = await _movieService.UpdateAsync(movie);

        if (movie is null)
            return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute]Guid id)
    {
        var isDeleted = await _movieService.DeleteByIdAsync(id);
        if (!isDeleted)
            return NotFound();

        return NoContent();
    }
}
