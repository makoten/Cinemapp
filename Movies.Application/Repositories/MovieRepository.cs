using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie)
    {
        var movieCreationSql = """
            insert into movies (id, slug, title, yearofrelease)
            values (@Id, @Slug, @Title, @YearOfRelease)
            """;
        var genreCreationSql = """
            insert into genres (movieId, name)
            values (@MovieId, @Name)
            """;

        var result = 0;
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            result = await connection.ExecuteAsync(movieCreationSql, movie, transaction);

            if (result > 0)
                foreach (var genre in movie.Genres)
                    await connection.ExecuteAsync(genreCreationSql, new { MovieId = movie.Id, Name = genre },
                        transaction);

            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }


        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var getMovieSql = """
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid and myr.userid = @userId
            where id = @id
            group by id, userrating
            """;
        var getGenresSql = """
            select name from genres
            where movieId = @id
            """;
        Movie? movie = null;
        try
        {
            movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(getMovieSql, new { id, userId }, transaction, cancellationToken: token));
            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(getGenresSql, new { id }, transaction, cancellationToken: token));
            foreach (var genre in genres) movie.Genres.Add(genre);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }

        return movie;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = 0;

        try
        {
            await connection.ExecuteAsync("""
                delete from genres
                where movieId = @id
                """, new { id }, transaction);

            result = await connection.ExecuteAsync("""
                delete from movies
                where id = @id
                """, new { id }, transaction);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

        var orderClause = string.Empty;
        if (options.SortField is not null)
            // this gets validated before this point, no fear of SQL injection attack
            orderClause = $"""
                    ,m.{options.SortField} 
                    order by m.{options.SortField} {(options.SortOrder == SortOrder.Descending ? "desc" : "asc")}
                """;

        var result = await connection.QueryAsync(new CommandDefinition($"""
            select m.*, 
                string_agg(distinct g.name, ',') as genres, 
                round(avg(r.rating), 1) as rating,
                myr.rating as userrating
            from movies m 
            left join genres g on m.id = g.movieid
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid and myr.userid = @userId
            where (@title is null or m.title like ('%' || @title || '%'))
            and (@yearofrelease is null or m.yearofrelease = @yearofrelease)
            group by id, userrating {orderClause}
            """, new
        {
            userId = options.UserId,
            title = options.Title,
            yearofrelease = options.YearOfRelease
        }, cancellationToken: token));

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(',')),
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating
        });
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token, Guid? userId)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var getMovieSql = """
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid and myr.userid = @userId
            where slug = @slug
            group by id, userrating
            """;
        var getGenresSql = """
            select name from genres
            where movieId = @id
            """;
        Movie? movie = null;
        try
        {
            movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(getMovieSql, new { slug, userId }, transaction, cancellationToken: token));
            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(getGenresSql, new { id = movie.Id }, transaction, cancellationToken: token));
            foreach (var genre in genres) movie.Genres.Add(genre);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }

        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie, Guid? userId, CancellationToken token)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        var result = 0;
        try
        {
            await connection.ExecuteAsync("""
                delete from genres where movieid = @id
                """, new { id = movie.Id }, transaction);

            foreach (var genre in movie.Genres)
                await connection.ExecuteAsync("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre }, transaction);

            result = await connection.ExecuteAsync("""
                update movies set slug = @Slug,
                title = @Title,
                yearofrelease = @YearOfRelease
                where id = @Id
                """, movie, transaction);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("select count(1) from movies where id = @id", new { id })
        );
        return result;
    }
}