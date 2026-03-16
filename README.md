# World Monitor–Style Intelligence Dashboard

A deployed dashboard that aggregates news, shows a global event map, and generates an AI intelligence brief through a backend-connected Ollama model.

## Live URLs
- App: `http://localhost:4200`
- API: `http://localhost/5087/api`
- LLM Host (internal): `http://127.0.0.1:11434`

## Reviewer
- GitHub reviewer: `@enghamzasalem`

## Architecture Summary
- **Frontend:** Angular SPA
- **Backend:** ASP.NET Core Web API
- **Database:** SQLite (persistent on server disk)
- **LLM:** Ollama running on the VPS
- **Reverse proxy / static hosting:** Nginx

## Implemented Features
- Aggregated news feed by source/category
- Interactive event map with live markers
- AI-generated intelligence brief from backend via Ollama
- Persistent database for feed sources, articles, briefs, and map events

## API Endpoints
- `GET /api/news`
- `POST /api/news/ingest`
- `GET /api/map`
- `GET /api/map/categories`
- `GET /api/brief`
- `POST /api/brief/generate`

## Local Run

### 1) Frontend
```bash
cd world-monitor-ui
npm install
npm run start
```

### 2) Backend
```bash
cd WorldMonitor.Api
dotnet restore
dotnet ef database update
dotnet run
```

### 3) Ollama
Install Ollama, start the service, and pull the model used by the API:
```bash
curl -fsSL https://ollama.com/install.sh | sh
ollama pull qwen2:1.5b
```

## Environment Variables
See `.env.example`.

## Deployment Summary
- Angular built with production build and served by Nginx
- ASP.NET Core API published for Linux and run as a systemd service
- SQLite database stored under `/opt/worldmonitor/data/worldmonitor.db`
- Ollama installed on the same VPS and accessed internally by the API

## LLM Server Notes
- Ollama is installed on the VPS
- Model used: `qwen2:1.5b`
- API calls Ollama internally via `http://127.0.0.1:11434`

## Evidence
- Screenshot: `docs/screenshots/dashboard.png`
- Optional video: `<add link here>`

## Submission Checklist
- [x] DB in use
- [x] API: feed digest, map data, AI brief
- [x] Frontend: dashboard with news + map + AI brief
- [x] App deployed and live
- [x] Ollama deployed on server and used by API
- [x] Reviewer added
- [x] PR includes architecture summary, live URL, screenshot/video
