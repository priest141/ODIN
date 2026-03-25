// src/components/TacticalMap.tsx
import * as React from 'react'; // Use Namespace import
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

import { meshtasticApi, osintApi } from '../api/services';
import type { MeshtasticNode, OsintEvent } from '../models/types';

// Icons outside the component
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

const DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});
L.Marker.prototype.options.icon = DefaultIcon;

export default function TacticalMap() {
    // Explicitly use React.useState to avoid any scope confusion
    const [nodes, setNodes] = React.useState<MeshtasticNode[]>([]);
    const [events, setEvents] = React.useState<OsintEvent[]>([]);

    const center: [number, number] = [-22.4149, -47.5614];

    React.useEffect(() => {
        let isMounted = true;
        const load = async () => {
            try {
                const [n, e] = await Promise.all([
                    meshtasticApi.getActiveNodes(),
                    osintApi.getNearbyEvents(center[0], center[1])
                ]);
                if (isMounted) {
                    setNodes(n);
                    setEvents(e);
                }
            } catch (err) {
                console.error("Map Data Error:", err);
            }
        };
        load();
        return () => { isMounted = false; };
    }, []);

    return (
        <div style={{ height: '100%', width: '100%', position: 'relative' }}>
            <MapContainer
                center={center}
                zoom={13}
                style={{ height: '100vh', width: '100vw' }}
            >
                <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />

                {nodes.map(n => n.latitude && (
                    <Marker key={n.nodeId} position={[n.latitude, n.longitude!]}>
                        <Popup>{n.longName}</Popup>
                    </Marker>
                ))}
            </MapContainer>
        </div>
    );
}