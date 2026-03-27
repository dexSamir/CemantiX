import Constants from 'expo-constants';
import { Platform } from 'react-native';

// For physical devices or simple local testing with Expo, you need the actual IP of the machine
// Or 10.0.2.2 for Android Emulator, localhost for iOS simulator.
const getBaseUrl = (): string => {
  // Can be configured via env
  if (__DEV__) {
    // If you want to test remote server even in dev mode, return the production link:
    return 'https://semantix-api.onrender.com/api';
  }
  return 'https://semantix-api.onrender.com/api'; // Prod URL
};

export const API_URL = getBaseUrl();

export interface JoinRoomResult {
    success: boolean;
    roomCode?: string;
    errorMessage?: string;
    room?: any;
}

export const safeFetch = async (url: string, options?: RequestInit) => {
  const response = await fetch(url, options);
  const text = await response.text();
  
  let data;
  try {
    data = JSON.parse(text);
  } catch (e) {
    // If it's not JSON, handle as text
    if (!response.ok) {
      throw new Error(text || `Server xətası (${response.status})`);
    }
    return text;
  }

  if (!response.ok) {
    throw new Error(data.error || data.message || data.errorMessage || `Xəta (${response.status})`);
  }
  return data;
};

export const registerPlayer = async (username: string) => {
  return safeFetch(`${API_URL}/player/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username }),
  });
};

export const createRoom = async (gameMode: number, hostPlayerId: string, isPrivate: boolean = false) => {
  return safeFetch(`${API_URL}/game/create-room`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ gameMode, hostPlayerId, isPrivate }),
  });
};
