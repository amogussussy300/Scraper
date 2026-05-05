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
- **Docker Desktop** — https://www.docker.com/products/docker-desktop (must be running, Linux containers mode)
- **git**

## Workflow

### 1. Clone and prepare local env file

```powershell
git clone https://github.com/amogussussy300/Scraper.git
cd Scraper
git switch sources-page
Copy-Item .env.example .env
```

### 2. Edit `.env`

Open `.env` and replace the placeholder with your own password. 

### 3. Start Postgres and configure secrets

```powershell
docker compose up -d
docker ps                      # status should show (healthy)

# Replace <same-as-.env> with the password you set in .env
dotnet user-secrets set "ConnectionStrings:ScraperDb" "Host=localhost;Port=5434;Database=scraper;Username=scraper;Password=<same-as-.env>" --project Scraper

# Verify
dotnet user-secrets list --project Scraper
```

### 4. Apply migrations

EF Core needs both projects: `--project` is where migrations live (`Scraper.Core`), `--startup-project` is where DI/config is read from (`Scraper`).

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project Scraper.Core --startup-project Scraper
```

### 5. Run the app

```powershell
dotnet run --project Scraper
```

## Common commands

| Task | Command |
|------|---------|
| Stop the DB (keep data) | `docker compose down` |
| Stop the DB and **wipe all data** | `docker compose down -v` |
| Inspect tables | `docker compose exec db psql -U scraper -d scraper -c "\dt"` |
| Open psql shell | `docker compose exec db psql -U scraper -d scraper` |
| View Postgres logs | `docker compose logs db --tail 50` |
| Add a new EF migration | `dotnet ef migrations add <Name> --project Scraper.Core --startup-project Scraper` |
| Apply pending migrations | `dotnet ef database update --project Scraper.Core --startup-project Scraper` |

## Troubleshooting

- **`28P01: password authentication failed`** — `.env` and User Secrets don't match (or volume was initialized with a different password). Fix: `docker compose down -v`, verify `.env` matches User Secrets, `docker compose up -d`, retry migration.
- **`role "scraper" does not exist`** — your connection landed on a different Postgres on the same host port. Stop the rogue service or change the host port in `docker-compose.yml` + User Secrets.
- **`POSTGRES_PASSWORD must be set in .env`** — `.env` is missing, named `.env.txt`, has a typo, or has a UTF-8 BOM. Inspect with `Get-Content .env | Format-Hex`.
- **VS shows no startup project** — open `Scraper.slnx` (File → Open → Project/Solution), not the folder.
