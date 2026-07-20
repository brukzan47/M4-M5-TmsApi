# M4–M5 branches

```bash
git init
git branch -M main
git add .
git commit -m "chore: create TmsApi project"
git remote add origin https://github.com/YOUR-USERNAME/TmsApi.git
git push -u origin main

git switch -c m4-session-1
git add .
git commit -m "feat: add middleware authentication and correlation logging"
git push -u origin m4-session-1

git switch -c m4-session-2
git add .
git commit -m "feat: add DI options and structured logging"
git push -u origin m4-session-2

git switch -c m4-session-3
git add .
git commit -m "feat: add controllers CRUD ProblemDetails and Scalar"
git push -u origin m4-session-3

git switch -c m5-session-1
git add .
git commit -m "feat: add EF Core PostgreSQL persistence"
git push -u origin m5-session-1

git switch -c m5-session-2
git add .
git commit -m "feat: add entity configurations relationships and reports"
git push -u origin m5-session-2
```

## EF commands

```bash
dotnet tool install --global dotnet-ef
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations list

# After configuration refinements:
dotnet ef migrations add RefineTmsModel
dotnet ef database update
```
