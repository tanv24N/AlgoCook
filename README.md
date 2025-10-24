
# AI Recipe Finder (.NET + React + SQLite) — Free & Deployable

**Tech:** .NET 8 Web API, EF Core + SQLite, React + Vite, TheMealDB (free API), GitHub Actions, GitHub Pages, Render (free web service).  
**What it does:** Lets users search recipes by **cuisine** or **ingredients**, view details, and **save favorites** (stored in SQLite on the API).

---

## 1) Local Setup (Everything Free)

### Prereqs
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js 18+](https://nodejs.org/) and npm
- (Optional) [Git](https://git-scm.com/)
- (Optional) [SQLite CLI](https://www.sqlite.org/download.html)

### Clone & Run
```bash
git clone <your-fork-or-repo-url> ai-recipe-finder
cd ai-recipe-finder

# 1) Back-end API
cd src/Api
dotnet restore
dotnet ef database update
dotnet run
# API will listen on http://localhost:5089 by default

# 2) Front-end
cd ../../src/Web
npm install
# create a .env file to point to your API (dev)
echo "VITE_API_BASE=http://localhost:5089" > .env
npm run dev
# open the shown localhost URL (usually http://localhost:5173)
```

SQLite DB file `recipes.db` is created in `src/Api` on first run.  
To inspect favorites:
```bash
cd src/Api
sqlite3 recipes.db "select * from Favorites;"
```

---

## 2) Project Structure

```
ai-recipe-finder/
  src/
    Api/            # .NET 8 Web API (C#, EF Core SQLite, CORS)
    Web/            # React + Vite frontend
  .github/workflows/
    api-ci.yml      # API CI (build/test)
    web-pages.yml   # Frontend GH Pages deploy
  README.md
```

---

## 3) Free Deployment

### A) Deploy API on Render (free web service)
1. Push this project to GitHub (public).
2. Go to https://render.com/ , create a free account.
3. New + Web Service → Build & deploy from a Git repository
   - Connect to your repo.
   - Root Directory: `src/Api`
   - Runtime: Docker (uses included `Dockerfile`), or use Native with:
     - Build Command: `dotnet restore && dotnet publish -c Release -o out`
     - Start Command: `dotnet out/Api.dll`
   - Free Plan.
4. Add environment variable (optional):
   - ASPNETCORE_URLS = http://0.0.0.0:8080
5. Deploy. After successful deploy you'll get a public URL like `https://your-api.onrender.com`.

Note: Render free instances may sleep when idle; cold starts are fine for demos.

### B) Deploy Frontend on GitHub Pages
1. In `src/Web/.env`, set the API base URL to your Render API:
   ```
   VITE_API_BASE=https://your-api.onrender.com
   ```
2. Commit & push.
3. In GitHub: Settings → Pages → Build and deployment → Source: GitHub Actions.
4. The included workflow `web-pages.yml` builds and publishes to `gh-pages` branch automatically on push to main.  
   Your site will be at: `https://<your-username>.github.io/<your-repo>/`

---

## 4) How it Works (Architecture)

- **Frontend (React + Vite)**:  
  Simple UI with two tabs: Cuisine or Ingredients. Users select a cuisine from a dropdown (fetched from API) or type ingredients (comma separated). Results grid shows recipe cards. Favorites page lets you manage saved recipes (via API).

- **Backend (ASP.NET Core minimal API)**:  
  Proxies the free TheMealDB API to avoid CORS and to unify response shapes.
  - GET /api/cuisines
  - GET /api/recipes/by-cuisine?cuisine=Italian
  - GET /api/recipes/by-ingredients?ingredients=chicken,tomato
  - GET /api/recipes/{id} (details)
  - GET /api/favorites, POST /api/favorites, DELETE /api/favorites/{id} (SQLite via EF Core)

- **Database (SQLite + EF Core)**:  
  Tracks favorites so you have a real SQL component.

- **External API (TheMealDB)**:  
  No API key needed. Docs: https://www.themealdb.com/api.php

---

## 5) Useful Commands

```bash
# API
cd src/Api
dotnet build
dotnet ef migrations add Init
dotnet ef database update

# Web
cd src/Web
npm run build
npm run preview
```

Generated on 2025-10-24
