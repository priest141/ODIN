using NetTopologySuite.Geometries;
using odin_Domain.Entities;
using odin_Domain.Enums;
using odin_Infrastructure.Data;
using System.Text.Json;

namespace odin_Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(OsintDbContext context)
    {
        // Ensure the database is created and migrations are applied
        context.Database.EnsureCreated();

        // If we already have nodes, the database has been seeded
        if (context.MeshtasticNodes.Any()) return;

        // Base Coordinates: Rio Claro, SP (-22.4149, -47.5614)
        // PostGIS uses (Longitude, Latitude) for Points
        var basePoint = new Point(-47.5614, -22.4149) { SRID = 4326 };
        var argoPoint = new Point(-47.5500, -22.4200) { SRID = 4326 };
        var arianePoint = new Point(-47.5700, -22.4100) { SRID = 4326 };
        var gaelPoint = new Point(-47.3800, -22.1800) { SRID = 4326 }; // Slightly further out towards Leme

        // 1. Seed Meshtastic Nodes
        var baseStation = new MeshtasticNode { NodeId = "!11111111", LongName = "Home_Base", ShortName = "BASE", BatteryLevel = 100, Voltage = 5.0m, Location = basePoint };
        var argoNode = new MeshtasticNode { NodeId = "!22222222", LongName = "Argo_Mobile_Server", ShortName = "ARGO", BatteryLevel = 85, Voltage = 12.4m, Location = argoPoint };
        var arianeNode = new MeshtasticNode { NodeId = "!33333333", LongName = "Ariane_GoBag", ShortName = "ARIA", BatteryLevel = 92, Voltage = 4.1m, Location = arianePoint };
        var gaelNode = new MeshtasticNode { NodeId = "!44444444", LongName = "Gael_Backpack", ShortName = "GAEL", BatteryLevel = 78, Voltage = 3.9m, Location = gaelPoint };

        context.MeshtasticNodes.AddRange(baseStation, argoNode, arianeNode, gaelNode);
        context.SaveChanges();

        // 2. Seed Meshtastic Messages
        var msg1 = new MeshtasticMessage { SenderId = arianeNode.NodeId, Payload = "Testing the mesh from downtown.", Snr = 5.5m };
        var msg2 = new MeshtasticMessage { SenderId = argoNode.NodeId, Payload = "Argo node online. SOPHIA scanner running.", Snr = 8.2m };

        context.MeshtasticMessages.AddRange(msg1, msg2);

        // 3. Seed OSINT Events
        // Simulated wildfire 15km away
        var fireLocation = new Point(-47.6000, -22.5000) { SRID = 4326 };
        var wildfireEvent = new OsintEvent
        {
            SourceName = "NASA_FIRMS",
            EventCategory = "Wildfire",
            Severity = EventSeverity.High,
            Description = "Thermal anomaly detected by VIIRS sensor.",
            Location = fireLocation,
            RawData = JsonDocument.Parse("{\"satellite\": \"Suomi NPP\", \"confidence\": \"high\", \"frp\": 45.2}"),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Simulated global cyber threat (No specific point location)
        var cyberEvent = new OsintEvent
        {
            SourceName = "CISA_Alerts",
            EventCategory = "Cybersecurity",
            Severity = EventSeverity.Critical,
            Description = "Zero-day vulnerability actively exploited in widespread infrastructure.",
            RawData = JsonDocument.Parse("{\"cve\": \"CVE-2026-1234\", \"target\": \"Industrial Control Systems\"}")
        };

        context.OsintEvents.AddRange(wildfireEvent, cyberEvent);

        // 4. Seed SIGINT Captures
        var sigintCapture = new SigintCapture
        {
            Type = CaptureType.WifiMac,
            Identifier = "00:1A:2B:3C:4D:5E",
            CaptureLocation = argoPoint, // Captured by the car
            Metadata = JsonDocument.Parse("{\"SSID\": \"Unknown_Network\", \"RSSI\": -65, \"Encryption\": \"WPA2\"}")
        };

        context.SigintCaptures.Add(sigintCapture);

        context.SaveChanges();
    }
}
