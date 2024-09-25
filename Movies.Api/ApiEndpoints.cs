﻿namespace Movies.Api;

/// <summary>
///     Class <c>ApiEndpoints</c> centralizes URI definitions by providing constants, to make for ease of adjustments.
/// </summary>
public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Movies
    {
        private const string Base = $"{ApiBase}/movies";

        public const string Create = Base;

        public const string Get = $"{Base}/{{idOrSlug}}";

        public const string GetAll = Base;

        public const string Update = $"{Base}/{{id:guid}}";

        public const string Delete = $"{Base}/{{id:guid}}";

        public const string Rate = $"{Base}/{{movieId:guid}}/ratings";

        public const string DeleteRating = $"{Base}/{{movieId:guid}}/ratings";
    }

    public static class Ratings
    {
        private const string Base = $"{ApiBase}/ratings";

        public const string GetUserRatings = $"{Base}/me";
    }
}