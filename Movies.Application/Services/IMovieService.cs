﻿
using Movies.Application.Models;

namespace Movies.Application.Services;


public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie);

    Task<Movie?> GetByIdAsync(Guid id, CancellationToken token);

    Task<Movie?> GetBySlugAsync(string slug, CancellationToken token);

    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token);

    Task<Movie?> UpdateAsync(Movie movie);

    Task<bool> DeleteByIdAsync(Guid id);
}
