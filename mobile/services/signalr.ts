import * as signalR from '@microsoft/signalr';
import { Platform } from 'react-native';

const getHubUrl = (): string => {
    if (__DEV__) {
        return 'https://semantix-api.onrender.com/hubs/game';
    }
    return 'https://semantix-api.onrender.com/hubs/game'; 
};

export class SignalRService {
    private connection: signalR.HubConnection;
    private static instance: SignalRService;

    private constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(getHubUrl())
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }

    public static getInstance(): SignalRService {
        if (!SignalRService.instance) {
            SignalRService.instance = new SignalRService();
        }
        return SignalRService.instance;
    }

    public async connect(): Promise<void> {
        if (this.connection.state === signalR.HubConnectionState.Disconnected) {
            try {
                await this.connection.start();
                console.log('SignalR connected.');
            } catch (err) {
                console.error('SignalR connection error: ', err);
            }
        }
    }

    public async register(username: string): Promise<void> {
        await this.connection.invoke('Register', username);
    }

    public async createRoom(gameMode: number, isPrivate: boolean): Promise<void> {
        await this.connection.invoke('CreateRoom', gameMode, isPrivate);
    }

    public async joinRoom(roomCode: string): Promise<void> {
        await this.connection.invoke('JoinRoom', roomCode);
    }

    public async submitGuess(roomCode: string, word: string): Promise<void> {
        await this.connection.invoke('SubmitGuess', roomCode, word);
    }
    
    public async setReady(roomCode: string): Promise<void> {
        await this.connection.invoke('SetReady', roomCode);
    }

    // Callbacks
    public on(eventName: string, callback: (...args: any[]) => void) {
        this.connection.on(eventName, callback);
    }

    public off(eventName: string, callback: (...args: any[]) => void) {
        this.connection.off(eventName, callback);
    }
    
    public async disconnect(): Promise<void> {
        if (this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.stop();
        }
    }
}

export default SignalRService.getInstance();
