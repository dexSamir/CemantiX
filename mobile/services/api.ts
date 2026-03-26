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

export const registerPlayer = async (username: string) => {
  const response = await fetch(`${API_URL}/player/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username }),
  });
  if (!response.ok) throw new Error('Qeydiyyat xətası');
  return response.json();
};

export const createRoom = async (gameMode: number, hostPlayerId: string, isPrivate: boolean = false) => {
  const response = await fetch(`${API_URL}/game/create-room`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ gameMode, hostPlayerId, isPrivate }),
  });
  if (!response.ok) throw new Error('Otaq yaradılarkən xəta');
  return response.json();
};
