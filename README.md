# Scraper

...

## Project layout

...

## Requirements

- **.NET 10 SDK** — https://dotnet.microsoft.com/download
- **EF Core CLI tool** (only needed if you want to create or apply migrations):
  ```powershell
  dotnet tool install --global dotnet-ef
  ```
  Verify with: `dotnet ef --version`
- **Docker Desktop** - https://www.docker.com/products/docker-desktop
- **git**

## workflow

1.

```
git clone https://github.com/amogussussy300/Scraper.git
cd Scraper
git switch sources-page
Copy-Item .env.example .env
```

2. edit .env var to your real password

3.

```
docker compose up -d
docker ps # status - healthy

# ! replace with your own password 
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5434;Database=scraper;Username=scraper;Password=<same-as-.env>" --project Scraper

# for verification
dotnet user-secrets list --project Scraper

$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project Scraper
```

4.

```
dotnet run --project Scraper
```