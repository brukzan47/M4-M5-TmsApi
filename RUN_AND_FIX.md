# Run the fixed M4–M5 TmsApi project

The package versions are aligned to prevent `NU1605`:

- Microsoft.EntityFrameworkCore: 10.0.10
- Microsoft.EntityFrameworkCore.Design: 10.0.10
- Microsoft.EntityFrameworkCore.Tools: 10.0.10
- Microsoft.AspNetCore.OpenApi: 10.0.10
- Microsoft.OpenApi: 2.7.5
- Npgsql.EntityFrameworkCore.PostgreSQL: 10.0.0
- Scalar.AspNetCore: 2.16.15

## Windows Command Prompt

```cmd
dotnet --version
dotnet clean
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
dotnet nuget locals all --clear
dotnet restore
dotnet build
```

Update the PostgreSQL password in `appsettings.json`, then run:

```cmd
dotnet tool update --global dotnet-ef --version 10.0.10
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run --urls "http://localhost:5000"
```

Open:

```text
http://localhost:5000/scalar/v1
```

## Push the correction

```cmd
git add .
git commit -m "fix: align EF Core and OpenAPI package versions"
git push
```
