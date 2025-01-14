using System.Text;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using MyHTTPServer.Sessions;
using System.IO;
using System.Collections.Generic;

namespace MyHTTPServer.EndPoints
{
    public class IndexEndpoint : BaseEndPoint
    {
        [Get("index")]
        public IHttpResponceResult GetIndex()
        {
            // Подключение к базе данных
            var ormContext = new ORMContext("Host=localhost; Port=5433; Username=postgres; Password=1903; Database=postgres");

            // Получаем данные для разных категорий
            var seriesList = ormContext.GetAllSeries();  // Получаем сериалы
            var moviesList = ormContext.GetAllMovies();
            var fightersList = ormContext.GetAllFighters();  // Получаем боевики
            var dramasList = ormContext.GetAllDramas();      // Получаем драмы
            var comediesList = ormContext.GetAllComedies();  // Получаем комедии

            // Чтение шаблона HTML
            var filePath = @"Templates/Pages/Dashboard/index.html";
            var fileContent = File.ReadAllText(filePath);
            
            var isAdmin = SessionStorage.IsAdmin(Context);
            var isAuthorized = SessionStorage.IsAuthorized(Context);

            // Генерация HTML для каждого типа контента
            fileContent = fileContent.Replace("{{SERIES_CARDS}}", GenerateHtmlForCards(seriesList, "series"));
            fileContent = fileContent.Replace("{{MOVIES_CARDS}}", GenerateHtmlForCards(moviesList, "movie"));
            fileContent = fileContent.Replace("{{FIGHTERS_CARDS}}", GenerateHtmlForCards(fightersList, "action-movie"));
            fileContent = fileContent.Replace("{{DRAMAS_CARDS}}", GenerateHtmlForCards(dramasList, "drama"));
            fileContent = fileContent.Replace("{{COMEDIES_CARDS}}", GenerateHtmlForCards(comediesList, "thrillers"));

            // Вставка кнопки навигации в зависимости от авторизации
            fileContent = UpdateNavbarButton(fileContent, isAdmin, isAuthorized);

            // Вставка информации о статусе администратора
            fileContent = fileContent.Replace("{{IsAdmin}}", isAdmin.ToString().ToLower());

            return Html(fileContent);
        }

        // Метод для генерации HTML-кода карточек для различных типов контента
        private string GenerateHtmlForCards(IEnumerable<dynamic> items, string type)
        {
            var html = new StringBuilder();
            foreach (var item in items)
            {
                // Формирование HTML-кода карточки
                html.Append($@"
                        <div class='{type}-card'>
                            <a href='http://localhost:6529/movie?id={item.Id}'>
                                <img src='{item.Image}' alt='{item.Title}'/>  
                            </a>
                            <div class='{type}-card-description'>
                                <h3>{item.Title}</h3>
                                <h6>{item.Year}</h6>
                            </div>
                        </div>");
            }
            return html.ToString();
        }

        // Метод для обновления кнопок навигации в зависимости от авторизации
        private string UpdateNavbarButton(string fileContent, bool isAdmin, bool isAuthorized)
        {
            if (isAuthorized)
            {
                if (isAdmin)
                {
                    fileContent = fileContent.Replace("{{NAVBAR_BUTTON}}", "<a href='/admin-dashboard'><button>Админ</button></a>");
                }
                else
                {
                    fileContent = fileContent.Replace("{{NAVBAR_BUTTON}}", "<a href='/profile'><button>Профиль</button></a>");
                }
            }
            else
            {
                fileContent = fileContent.Replace("{{NAVBAR_BUTTON}}", "<a href='/login'><button>Войти</button></a>");
            }

            return fileContent;
        }
    }
}
