// --- MESHTASTIC DOMAIN ---
export interface MeshtasticNode {
    nodeId: string;
    longName: string;
    shortName: string;
    batteryLevel?: number;
    voltage?: number;
    lastHeard: string; // ISO 8601 Date String
    latitude?: number;
    longitude?: number;
}

export interface MeshtasticMessage {
    id: string;
    senderId: string;
    senderName: string;
    receiverId?: string;
    payload: string;
    snr?: number;
    timestamp: string;
}

// --- OSINT DOMAIN ---
export type EventSeverity = 'Info' | 'Low' | 'Medium' | 'High' | 'Critical';

export interface OsintEvent {
    id: string;
    sourceName: string;
    eventCategory: string;
    severity: EventSeverity;
    description: string;
    rawData: any; // Mapped from Postgres JSONB
    latitude?: number;
    longitude?: number;
    discoveredAt: string;
    expiresAt?: string;
}

// --- SIGINT DOMAIN ---
export type CaptureType = 'WifiMac' | 'BluetoothMac' | 'RfSignal' | 'Imsi';

export interface SigintCapture {
    id: string;
    type: CaptureType;
    identifier: string;
    metadata: any; // Mapped from Postgres JSONB
    latitude?: number;
    longitude?: number;
    capturedAt: string;
}