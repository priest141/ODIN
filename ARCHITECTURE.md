# Odin Architecture Overview

The Odin platform is built on a strictly decoupled microservices architecture designed to run on low-power edge devices (like a Raspberry Pi or mobile car server) or a dedicated homelab.

## System Flow

1. **The Physical Layer (Mesh Network)**
   * Remote operators transmit GPS telemetry and encrypted text payloads over LoRa via the Meshtastic protocol.
   * A base station radio receives the packets and passes them over USB/Serial to the host machine.

2. **The Ingestion Layer (Python Worker)**
   * `osint_meshtastic_worker` runs continuously in the background.
   * It uses `PyPubSub` to listen to the serial port.
   * Incoming packets are mapped to Python domain models and injected directly into the PostGIS database using raw SQL `ST_MakePoint` commands to ensure spatial integrity.

3. **The Data Layer (PostGIS)**
   * Acts as the single source of truth.
   * Handles complex spatial mathematics natively (e.g., calculating distance between nodes and OSINT events).

4. **The Gateway Layer (.NET 10 API)**
   * Read-only interface for the frontend.
   * Translates native PostGIS Geometries into flattened JSON objects using Entity Framework Core.
   * Enforces CORS policies and routes traffic.

5. **The Visualization Layer (React/Vite)**
   * Polls the API Gateway.
   * Renders the data over offline-capable Leaflet maps utilizing CartoDB dark matter tiles for tactical visualization.