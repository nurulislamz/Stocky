# SQLite script to add a new migration
dotnet ef migrations add InitialCreate --project stockymodels --startup-project stockyapi

# SQlite requires you to set UserSecrets like below: 
dotnet user-secrets set ConnectionStrings:SqliteConnection "Data Source=C:\Users\nurul\source\repos\Stocky\stockydb.db" --project stockyapi/stockyapi.csproj

# Scripts to migrate 
dotnet ef database update --project stockymodels --startup-project stockyapi

# Scripts to show SQL migration script, you can manipulate this for a later migration
dotnet ef migrations script --project stockymodels --startup-project stockyapi > stockymodels/Migrations/SQL/20260123115052_InitialCrea