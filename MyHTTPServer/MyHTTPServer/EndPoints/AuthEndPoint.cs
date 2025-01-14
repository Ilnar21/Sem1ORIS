using Npgsql;
using System;
using System.Net;
using System.Text;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using MyHTTPServer.Sessions;
using HttpServerLibrary.HttpResponce;

namespace MyHTTPServer.EndPoints;

public class AuthEndPoint : BaseEndPoint
{
    // Эндпоинт для получения страницы входа (GET /login)
    [Get("login")]
    public IHttpResponceResult AuthGet()
    {
        if (SessionStorage.IsAuthorized(Context))
        {
            return Redirect("admin-dashboard");
        }

        var file = File.ReadAllText(@"Templates/Pages/Auth/login.html");
        return Html(file);
    }


    // Эндпоинт для обработки формы входа (POST /login)
    [Post("login")]
    public IHttpResponceResult Login(string mail, string password)
    {
        if (string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(password))
        {
            Context.Response.ContentType = "text/html; charset=utf-8";
            Context.Response.ContentEncoding = Encoding.UTF8;

            return Html("<p>Пожалуйста, заполните все поля</p>");
        }

        string connectionString = "Host=localhost; Port=5433; Username=postgres; Password=1903; Database=postgres";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT COUNT(*) FROM users WHERE email = @Email AND password = @Password";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", mail);
                command.Parameters.AddWithValue("@Password", password);

                int userCount = Convert.ToInt32(command.ExecuteScalar());
                if (userCount > 0)
                {
                    // Данные корректны, создаем сессию
                    var sessionToken = SessionStorage.GenerateNewToken();
                    var userId = "admin"; // или получите ID пользователя из БД

                    SessionStorage.SaveSession(sessionToken, userId);
                    var cookie = new Cookie("session-token", sessionToken) { Expires = DateTime.Now.AddDays(1) };
                    Context.Response.SetCookie(cookie);

                    return Redirect("/admin-dashboard");
                }
            }
        }

        // Если данные неверны
        var file = File.ReadAllText(@"Templates/Pages/Auth/login.html");
        return Html(file);
    }


}

