using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

public class ORMContext
{
    private readonly string _connectionString;

    public ORMContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Метод для получения данных о фильме по ID
    public MovieDetails GetMovieDetailsById(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Получаем основные данные о фильме
            var movieQuery = "SELECT * FROM movies WHERE id = @id";
            var movie = connection.QuerySingleOrDefault<Movie>(movieQuery, new { id });

            if (movie == null) return null;

            // Получаем актеров для этого фильма
            var actorsQuery = "SELECT * FROM actors WHERE movie_id = @movie_id";
            var actors = connection.Query<Actor>(actorsQuery, new { movie_id = id }).ToList();

            // Возвращаем все данные о фильме, включая актеров
            return new MovieDetails
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Year = movie.Year,
                Image = movie.Image,
                Trailer = movie.Trailer, 
                Actors = actors
            };
        }
    }

    // Универсальный метод для получения всех фильмов по жанру
    public List<Movie> GetMoviesByGenre(string genre)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT * FROM movies WHERE genre = @genre";
            return connection.Query<Movie>(query, new { genre }).ToList();
        }
    }

    // Пример для использования универсального метода
    public List<Movie> GetAllMovies()
    {
        return GetMoviesByGenre("Movies");
    }

    public List<Movie> GetAllSeries()
    {
        return GetMoviesByGenre("Serials");
    }

    public List<Movie> GetAllFighters()
    {
        return GetMoviesByGenre("Action");
    }

    public List<Movie> GetAllDramas()
    {
        return GetMoviesByGenre("Drama");
    }

    public List<Movie> GetAllComedies()
    {
        return GetMoviesByGenre("Comedy");
    }

    // Получение списка актеров
    public List<Actor> GetAllActors()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT * FROM actors";
            return connection.Query<Actor>(query).ToList();
        }
    }

    // Метод для получения актеров по фильму
    public List<Actor> GetActorsByMovieId(int movieId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT * FROM actors WHERE movie_id = @movieId";
            return connection.Query<Actor>(query, new { movieId }).ToList();
        }
    }
}


// Классы, которые будут использоваться для отображения данных

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
    
    public string Image { get; set; }
    public string Trailer { get; set; }
    public string Description { get; set; }
}

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public int MovieId { get; set; }  
}

public class MovieDetails : Movie
{
    public List<Actor> Actors { get; set; }
}
