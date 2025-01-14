using System.Net;
using System.Text;
using Npgsql;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using MyHTTPServer.Sessions;

namespace MyHTTPServer.EndPoints;

public class AdminDatabaseEndPoint : BaseEndPoint
{
    [Get("admin-dashboard")]
    public IHttpResponceResult AdminDashboard()
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            Console.WriteLine("Доступ запрещён");
            return Html("<h1>Доступ запрещён</h1>");
        }

        Console.WriteLine("Пользователь авторизован. Возвращаем админ-панель.");
        var htmlBuilder = new StringBuilder();
        htmlBuilder.Append("<!DOCTYPE html>");
        htmlBuilder.Append("<html lang='ru'>");
        htmlBuilder.Append("<head>");
        htmlBuilder.Append("<meta charset='UTF-8'>");
        htmlBuilder.Append("<title>Админ-панель</title>");
        htmlBuilder.Append("<style>");
        htmlBuilder.Append(@"
            body {
                font-family: Arial, sans-serif;
                background-color: #f0f8ff;
                margin: 0;
                padding: 0;
            }
            header {
                background-color: #0078D7;
                color: white;
                padding: 10px;
                text-align: center;
            }
            header h1 {
                margin: 0;
            }
            ul {
                list-style-type: none;
                padding: 0;
            }
            li {
                margin: 10px 0;
            }
            a {
                text-decoration: none;
                color: #0078D7;
                font-weight: bold;
            }
            a:hover {
                color: #005A9E;
            }
            .container {
                padding: 20px;
                max-width: 800px;
                margin: 0 auto;
            }
            .logout-btn {
                display: inline-block;
                margin-top: 20px;
                padding: 10px 20px;
                background-color: #D9534F;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }
            .logout-btn:hover {
                background-color: #C9302C;
            }
        ");
        htmlBuilder.Append("</style>");
        htmlBuilder.Append("</head>");
        htmlBuilder.Append("<body>");
        htmlBuilder.Append("<header>");
        htmlBuilder.Append("<h1>Админ-Панель APPLE TV</h1>");
        htmlBuilder.Append("</header>");
        htmlBuilder.Append("<div class='container'>");
        htmlBuilder.Append("<h2>Управление базами данных</h2>");
        htmlBuilder.Append("<ul>");
        htmlBuilder.Append("<li><a href='/admin/manage-table?table=movies'>Фильмы</a></li>");
        htmlBuilder.Append("<li><a href='/admin/manage-actors'>Актеры</a></li>");
        htmlBuilder.Append("</ul>");
        htmlBuilder.Append("<a class='logout-btn' href='/admin/logout'>Выйти</a>");
        htmlBuilder.Append("</div>");
        htmlBuilder.Append("</body>");
        htmlBuilder.Append("</html>");

        Context.Response.ContentType = "text/html; charset=utf-8";
        return Html(htmlBuilder.ToString());
    }

    [Get("admin/manage-table")]
    public IHttpResponceResult ManageTable(string table)
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            return Html("<h1>Доступ запрещён</h1>");
        }

        var rows = GetTableData(table);

        var htmlBuilder = new StringBuilder();
        htmlBuilder.Append("<!DOCTYPE html>");
        htmlBuilder.Append("<html lang='ru'>");
        htmlBuilder.Append("<head>");
        htmlBuilder.Append("<meta charset='UTF-8'>");
        htmlBuilder.Append("<title>Управление таблицей</title>");
        htmlBuilder.Append("</head>");
        htmlBuilder.Append("<body>");
        htmlBuilder.Append($"<h1>Управление таблицей: {table}</h1>");
        htmlBuilder.Append("<table border='1'>");
        htmlBuilder.Append("<tr><th>ID</th><th>Название</th><th>Описание</th><th>Категория</th><th>Год</th><th>Путь к картинке</th><th>Трейлер</th><th>Действия</th></tr>");

        foreach (var row in rows)
        {
            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td>{row[0]}</td>"); // ID
            htmlBuilder.Append($"<td>{row[1]}</td>"); // Название
            htmlBuilder.Append($"<td>{row[2]}</td>"); // Описание
            htmlBuilder.Append($"<td>{row[3]}</td>"); // Жанр
            htmlBuilder.Append($"<td>{row[4]}</td>"); // Год
            htmlBuilder.Append($"<td>{row[5]}</td>"); // Путь к картинке
            htmlBuilder.Append($"<td>{row[6]}</td>"); // Трейлер
            htmlBuilder.Append($"<td><a href='/admin/delete-movie?id={row[0]}'>Удалить</a></td>");
            htmlBuilder.Append($"</tr>");
        }

        htmlBuilder.Append("</table>");
        htmlBuilder.Append($"<form method='POST' action='/admin/add-row'>");
        htmlBuilder.Append($"<input type='hidden' name='table' value='{table}'>");
        htmlBuilder.Append("<input type='number' name='id' placeholder='ID' required>");
        htmlBuilder.Append("<input type='text' name='title' placeholder='Название' required>");
        htmlBuilder.Append("<input type='text' name='description' placeholder='Описание' required>");
        htmlBuilder.Append("<input type='text' name='genre' placeholder='Категория' required>");
        htmlBuilder.Append("<input type='number' name='year' placeholder='Год' required>");
        htmlBuilder.Append("<input type='text' name='image' placeholder='Путь к картинке' required>");
        htmlBuilder.Append("<input type='text' name='trailer' placeholder='Ссылка на трейлер' required>");
        htmlBuilder.Append("<button type='submit'>Добавить фильм</button>");
        htmlBuilder.Append("</form>");
        htmlBuilder.Append("</body>");
        htmlBuilder.Append("</html>");

        Context.Response.ContentType = "text/html; charset=utf-8";
        return Html(htmlBuilder.ToString());
    }

    [Post("admin/add-row")]
    public IHttpResponceResult AddRow(string table, int id, string title, string description, string genre, int year, string image, string trailer)
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            return Html("<h1>Доступ запрещён</h1>");
        }

        var allowedTables = new[] { "movies" };
        if (!allowedTables.Contains(table))
        {
            return Html("<h1>Недопустимая таблица</h1>");
        }

        var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=1903;Database=postgres;Encoding=UTF8";

        try
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                var sql = $@"
                    INSERT INTO {table} 
                    (id, title, description, genre, year, image, trailer) 
                    VALUES (@id, @title, @description, @genre, @year, @image, @trailer)";
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@genre", genre);
                    command.Parameters.AddWithValue("@year", year);
                    command.Parameters.AddWithValue("@image", image);
                    command.Parameters.AddWithValue("@trailer", trailer);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка добавления записи: {ex.Message}");
            return Html($"<h1>Ошибка: {ex.Message}</h1>");
        }

        return Redirect($"/admin/manage-table?table={table}");
    }
    
    [Get("admin/delete-movie")]
    public IHttpResponceResult DeleteMovie(int id)
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            return Html("<h1>Доступ запрещён</h1>");
        }

        var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=1903;Database=postgres;Encoding=UTF8";

        try
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // SQL запрос для удаления фильма по ID
                var sql = "DELETE FROM movies WHERE id = @id";
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка удаления фильма с ID {id}: {ex.Message}");
            return Html($"<h1>Ошибка: {ex.Message}</h1>");
        }

        // После удаления фильма перенаправляем на страницу управления таблицей
        return Redirect("/admin/manage-table?table=movies");
    }


    private List<List<object>> GetTableData(string table)
    {
        var rows = new List<List<object>>();
        var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=1903;Database=postgres;Encoding=UTF8";
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            var command = new NpgsqlCommand($"SELECT * FROM {table}", conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new List<object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetValue(i));
                    }
                    rows.Add(row);
                }
            }
        }
        return rows;
    }

    [Get("admin/manage-actors")]
    public IHttpResponceResult ManageActors()
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            return Html("<h1>Доступ запрещён</h1>");
        }

        var actors = GetAllActors();

        var htmlBuilder = new StringBuilder();
        htmlBuilder.Append("<!DOCTYPE html>");
        htmlBuilder.Append("<html lang='ru'>");
        htmlBuilder.Append("<head>");
        htmlBuilder.Append("<meta charset='UTF-8'>");
        htmlBuilder.Append("<title>Управление актерами</title>");
        htmlBuilder.Append("</head>");
        htmlBuilder.Append("<body>");
        htmlBuilder.Append("<h1>Управление актерами</h1>");
        htmlBuilder.Append("<table border='1'>");
        htmlBuilder.Append("<tr><th>Имя</th><th>Роль</th><th>Фильм</th><th>Действия</th></tr>");

        foreach (var actor in actors)
        {
            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td>{actor[0]}</td>"); // Имя
            htmlBuilder.Append($"<td>{actor[1]}</td>"); // Роль
            htmlBuilder.Append($"<td>{actor[2]}</td>"); // Название фильма
            htmlBuilder.Append($"<td><a href='/admin/delete-actor?actorName={actor[0]}'>Удалить</a></td>");
            htmlBuilder.Append($"</tr>");
        }

        htmlBuilder.Append("</table>");
        htmlBuilder.Append("<h2>Добавить нового актера</h2>");
        htmlBuilder.Append($"<form method='POST' action='/admin/add-actor'>");
        htmlBuilder.Append("<input type='text' name='name' placeholder='Имя актера' required>");
        htmlBuilder.Append("<input type='text' name='role' placeholder='Роль актера' required>");
        htmlBuilder.Append("<input type='text' name='movieTitle' placeholder='Фильм' required>");
        htmlBuilder.Append("<button type='submit'>Добавить актера</button>");
        htmlBuilder.Append("</form>");
        htmlBuilder.Append("</body>");
        htmlBuilder.Append("</html>");

        Context.Response.ContentType = "text/html; charset=utf-8";
        return Html(htmlBuilder.ToString());
    }

    [Post("admin/add-actor")]
    public IHttpResponceResult AddActor(string name, string role, string movieTitle)
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            return Html("<h1>Доступ запрещён</h1>");
        }

        var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=1903;Database=postgres;Encoding=UTF8";

        try
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                var sql = @"
                    INSERT INTO actors (name, role, movie_id)
                    VALUES 
                    (@name, @role, (SELECT id FROM movies WHERE title = @movieTitle))";
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@role", role);
                    command.Parameters.AddWithValue("@movieTitle", movieTitle);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка добавления актера: {ex.Message}");
            return Html($"<h1>Ошибка: {ex.Message}</h1>");
        }

        return Redirect("/admin/manage-actors");
    }

    [Get("admin/delete-actor")]
    public IHttpResponceResult DeleteActor(string actorName)
    {
        if (!SessionStorage.IsAdmin(Context))
        {
            return Html("<h1>Доступ запрещён</h1>");
        }

        Console.WriteLine($"Попытка удалить актера: {actorName}");

        actorName = actorName.Trim(); // Удаление пробелов

        var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=1903;Database=postgres;Encoding=UTF8";

        try
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                var sql = "DELETE FROM actors WHERE name = @actorName";
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@actorName", actorName);
                    var rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Количество удалённых строк: {rowsAffected}");

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Актер не найден.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка удаления актера {actorName}: {ex.Message}");
            return Html($"<h1>Ошибка: {ex.Message}</h1>");
        }

        return Redirect("/admin/manage-actors");
    }


    private List<List<object>> GetAllActors()
    {
        var actors = new List<List<object>>();
        var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=1903;Database=postgres;Encoding=UTF8";

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            var sql = @"
                SELECT actors.name, actors.role, movies.title 
                FROM actors
                JOIN movies ON actors.movie_id = movies.id";

            using (var command = new NpgsqlCommand(sql, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var actor = new List<object>
                        {
                            reader.GetString(0), // Имя
                            reader.GetString(1), // Роль
                            reader.GetString(2)  // Название фильма
                        };
                        actors.Add(actor);
                    }
                }
            }
        }

        return actors;
    }

    [Get("admin/logout")]
    public IHttpResponceResult Logout()
    {
        Console.WriteLine("[Logout] Выполняется выход");
        var cookie = new Cookie("session-token", "") { Expires = DateTime.Now.AddDays(-1) };
        Context.Response.SetCookie(cookie);
        SessionStorage.ClearSession();
        Console.WriteLine("[Logout] Редирект на страницу входа");
        return Redirect("http://localhost:6529/login");
    }
}
