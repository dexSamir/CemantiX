import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Text, ActivityIndicator, Alert } from 'react-native';
import { COLORS } from '../../constants/theme';
import { GuessInput } from '../../components/GuessInput';
import { GuessList, GuessItem } from '../../components/GuessList';
import { ColorBar } from '../../components/ColorBar';
import SignalRService from '../../services/signalr';

export default function BattleGame() {
  const [roomCode, setRoomCode] = useState<string | null>(null);
  const [guesses, setGuesses] = useState<GuessItem[]>([]);
  const [opponentGuesses, setOpponentGuesses] = useState<number>(0);
  const [isWon, setIsWon] = useState(false);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const initSignalR = async () => {
      await SignalRService.connect();
      await SignalRService.register('Orxan'); // Fake auth for demo
      setIsConnected(true);

      SignalRService.on('RoomCreated', (room) => {
        setRoomCode(room.roomCode);
        Alert.alert('Otaq Yaradıldı', `Otaq kodu: ${room.roomCode}\nDostunuza göndərin!`);
      });

      SignalRService.on('PlayerJoined', (data) => {
        Alert.alert('Oyunçu Qoşuldu', `${data.username} otağa qoşuldu!`);
        SignalRService.setReady(roomCode!);
      });

      SignalRService.on('RoundStarted', (round) => {
        Alert.alert('Raund Başladı!', 'Daha sürətli olan qazansın!');
      });

      SignalRService.on('GuessResult', (res) => {
        setGuesses(prev => [{
            id: res.word,
            word: res.word,
            rank: res.rank,
            colorCategory: res.colorCategory,
            isCorrect: res.isCorrect
        }, ...prev]);
        
        if (res.isCorrect) setIsWon(true);
      });

      SignalRService.on('OpponentGuessed', (res) => {
        setOpponentGuesses(prev => prev + 1);
      });

      SignalRService.on('RoundComplete', (res) => {
        setIsWon(true);
        Alert.alert('Raund Bitdi', res.message);
      });
    };

    initSignalR();

    return () => {
      SignalRService.disconnect();
    };
  }, []);

  const handleCreateRoom = () => {
    // 1 = Battle Mode
    SignalRService.createRoom(1, false);
  };

  const handleGuess = (word: string) => {
    if (roomCode) {
      SignalRService.submitGuess(roomCode, word);
    }
  };

  if (!isConnected) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.text}>Serverə qoşulur...</Text>
      </View>
    );
  }

  if (!roomCode) {
    return (
      <View style={styles.center}>
        <Text style={styles.title}>Battle Rejimi</Text>
        <Text style={styles.text} onPress={handleCreateRoom} style={styles.btn}>Otaq Yarat</Text>
      </View>
    );
  }

  const bestGuess = guesses.length > 0 ? guesses.reduce((prev, curr) => prev.rank < curr.rank ? prev : curr) : null;

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.statsText}>Sənin cəhdin: {guesses.length}</Text>
        <Text style={styles.statsText}>Rəqibin cəhdi: {opponentGuesses}</Text>
      </View>
      <View style={styles.header}>
        <Text style={styles.statsText}>Otaq Kodu: {roomCode}</Text>
      </View>
      
      {bestGuess && (
        <ColorBar rank={bestGuess.rank} colorCategory={bestGuess.colorCategory} />
      )}

      <GuessList guesses={guesses} />
      {!isWon && <GuessInput onSubmit={handleGuess} />}
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: COLORS.background },
  center: { flex: 1, justifyContent: 'center', alignItems: 'center', backgroundColor: COLORS.background },
  text: { color: COLORS.text, marginTop: 10 },
  title: { fontSize: 24, fontWeight: 'bold', color: COLORS.text, marginBottom: 20 },
  btn: { backgroundColor: COLORS.primary, padding: 15, borderRadius: 10, color: COLORS.background, fontWeight: 'bold' },
  header: { flexDirection: 'row', justifyContent: 'space-between', paddingHorizontal: 20, paddingTop: 10 },
  statsText: { color: COLORS.textSecondary, fontSize: 14, fontWeight: '500' }
});
