// src/api/client.ts
import axios from 'axios';

// Pointing to your Dockerized .NET 10 API
export const apiClient = axios.create({
    baseURL: 'http://localhost:8080/api/v1',
    headers: {
        'Content-Type': 'application/json',
    },
    // A 10-second timeout prevents the UI from hanging infinitely if the API goes down
    timeout: 10000,
});