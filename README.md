# ODIN: Tactical Overwatch Dashboard

Odin is a self-hosted, offline-capable tactical awareness and OSINT platform. It ingests live hardware telemetry from Meshtastic mesh networks, scrapes global OSINT threat feeds, and visualizes the intelligence on a dark-mode cartographic dashboard.

## 🚀 The Tech Stack
* **Frontend:** React + Vite + Tailwind CSS v4 + Leaflet (CartoDB Dark Matter)
* **API Gateway:** C# .NET 10 (Clean Architecture)
* **Spatial Core:** PostgreSQL + PostGIS 15
* **Ingestion Worker:** Python 3.11 + PyPubSub (Hardware Serial Interface)
* **Infrastructure:** Docker & Docker Compose

## 🛠️ Prerequisites
* Docker and Docker Compose installed.
* A physical Meshtastic device connected via USB or accessible via TCP on the local network.
* Node.js v22+ (for local frontend development).
* .NET 10 SDK (for local API development).

## ⚡ Quickstart (Docker Compose)

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd odin

2. **Configure the environment:**
Copy the example config and inject your secure variables.

```bash
cp .env.example .env
```
**CRITICAL: Ensure RADIO_PORT in your .env matches your physical hardware interface (e.g., /dev/ttyUSB0).**

3. **Spin up the stack:**
This will build the API, the Python worker, and pull the PostGIS database.

```bash
docker compose up -d --build
```
4. **Verify Systems:**
- Swagger API Docs: http://localhost:8080/swagger
- Hardware Worker Logs: 
```bash
docker logs -f osint_meshtastic_worker
```

5. **Start the Frontend (Local Dev):**
```bash
cd odin.Frontend
npm install
npm run dev
```
Dashboard available at: http://localhost:5173

## 📁 Project Structure
- /odin.Api - C# .NET 10 REST endpoints.
- /odin.Application - Business logic and interfaces.
- /odin.Domain - Core C# entities and Enums.
- /odin.Infrastructure - EF Core contexts and PostGIS integrations.
- /odin.Worker - Python scripts for physical radio ingestion.
- /odin.Frontend - React/Vite dashboard UI.