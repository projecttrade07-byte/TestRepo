# Gather Event Planner

A two-page event planner built with ASP.NET Core 8, Entity Framework Core, SQL Server, and plain HTML/CSS/JavaScript.

## Pages

- `/add-event.html` — create an event with its name, location, start/end time, and optional description.
- `/events.html` — view saved events in start-time order.

The JSON API is available at `GET /api/events`, `GET /api/events/{id}`, and `POST /api/events`.

## Run locally

1. Install the .NET 8 SDK and have SQL Server available locally.
2. Update `ConnectionStrings:EventsDatabase` in `appsettings.json` if your SQL Server connection differs. The default uses Windows integrated authentication and a local SQL Server instance.
3. From this directory, run:

   ```powershell
   dotnet run
   ```

4. Open the URL printed by ASP.NET Core. The app creates the `EventPlannerDb` database and its event table on first startup.

For development convenience, the database is initialized with `EnsureCreated`. If you change the schema after the database has been created, remove/recreate the development database or replace this with EF Core migrations before deploying.
