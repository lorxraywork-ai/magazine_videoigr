# magazine_videoigr

Учебное веб-приложение ASP.NET Core MVC на .NET 8 по теме:

**«Информационная система магазина видеоигр»**

## Что внутри

- `VideoGameStoreSystem.slnx` — решение проекта
- `VideoGameStoreSystem.Web/` — основной MVC-проект
- `VideoGameStoreSystem.Web/sql/` — SQL-скрипты создания БД и схемы
- `VideoGameStoreSystem.Web/README.md` — подробная инструкция по запуску

## Технологии

- ASP.NET Core MVC
- Razor Views
- EF Core
- SQL Server
- Cookie Authentication
- Bootstrap 5
- Chart.js
- ClosedXML

## Быстрый запуск

```powershell
cd .\VideoGameStoreSystem.Web
dotnet restore
dotnet build
dotnet run --launch-profile http
```

После запуска приложение обычно доступно по адресу:

`http://localhost:5231`

## Подробности

Полная инструкция по базе данных, демонстрационным учетным записям и структуре проекта находится в файле:

`VideoGameStoreSystem.Web/README.md`
