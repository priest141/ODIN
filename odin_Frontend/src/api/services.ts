// src/api/services.ts
import { apiClient } from './client';
import type { MeshtasticNode, OsintEvent, SigintCapture, EventSeverity, CaptureType } from '../models/types';

export const meshtasticApi = {
    getActiveNodes: async (activeWithinMinutes: number = 60): Promise<MeshtasticNode[]> => {
        const response = await apiClient.get<MeshtasticNode[]>('/Meshtastic/nodes/active', {
            params: { activeWithinMinutes }
        });
        return response.data;
    }
};

export const osintApi = {
    getNearbyEvents: async (lat: number, lon: number, radiusInMeters: number = 50000, minSeverity: EventSeverity = 'Info'): Promise<OsintEvent[]> => {
        const response = await apiClient.get<OsintEvent[]>('/Osint/nearby', {
            params: { lat, lon, radiusInMeters, minSeverity }
        });
        return response.data;
    }
};

export const sigintApi = {
    getRecentCaptures: async (type: CaptureType, count: number = 100): Promise<SigintCapture[]> => {
        const response = await apiClient.get<SigintCapture[]>('/Sigint/recent', {
            params: { type, count }
        });
        return response.data;
    }
};