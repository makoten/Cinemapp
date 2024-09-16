using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;


public class MovieRepository : IMovieRepository
{

    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

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
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {

            result = await connection.ExecuteAsync(movieCreationSql, movie, transaction);

            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(genreCreationSql, new { MovieId = movie.Id, Name = genre }, transaction);
                }
            }

            transaction.Commit();
        } 
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }


        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var getMovieSql = """
            select * from movies
            where id = @id
            """;
        var getGenresSql = """
            select name from genres
            where movieId = @id
            """;
        Movie? movie = null;
        try
        {
            movie = await connection.QuerySingleOrDefaultAsync<Movie>(getMovieSql, movie, transaction);
            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(getGenresSql, new { id = movie.Id }, transaction);

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
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
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
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

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync("""
            select m.*, string_agg(g.name, ',') as genres
            from movies m left join genres g on m.id = g.movieid
            group by id
            """);

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }



    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var getMovieSql = """
            select * from movies
            where slug = @slug
            """;
        var getGenresSql = """
            select name from genres
            where movieId = @id
            """;
        Movie? movie = null;
        try
        {
            movie = await connection.QuerySingleOrDefaultAsync<Movie>(getMovieSql, new { slug }, transaction);
            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(getGenresSql, new { id = movie.Id }, transaction);

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }

        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        var result = 0;
        try
        {
            await connection.ExecuteAsync("""
            delete from genres where movieid = @id
            """, new { id = movie.Id }, transaction);

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync("""
                insert into genres (movieId, name)
                values (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, transaction);
            }

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
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteScalarAsync<bool>("""
            select count(1) from movies where id = @id
            """, new { id });
        return result;
    }

}
