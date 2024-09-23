﻿namespace Movies.Contracts.Requests;

public class GetAllMoviesRequest
{
    public string? Title { get; init; }

    public int? Year { get; init; }

    public required string? SortBy { get; init; }
}