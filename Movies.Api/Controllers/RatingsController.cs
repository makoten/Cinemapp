﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class RatingsController(IRatingService ratingService) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid movieId, [FromBody] RateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var result = await ratingService.RateMovieAsync(movieId, userId!.Value, request.Rating, token);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid movieId, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var result = await ratingService.DeleteRatingAsync(movieId, userId!.Value, token);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetRatingsForUser(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
        var ratingsResponse = ratings.MapToResponse();
        return Ok(ratingsResponse);
    }
}