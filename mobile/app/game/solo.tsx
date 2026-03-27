import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Text, ActivityIndicator, Alert } from 'react-native';
import { COLORS } from '../../constants/theme';
import { GuessInput } from '../../components/GuessInput';
import { GuessList, GuessItem } from '../../components/GuessList';
import { ColorBar } from '../../components/ColorBar';
import { API_URL, createRoom, safeFetch } from '../../services/api';

export default function SoloGame() {
  const [roomId, setRoomId] = useState<string | null>(null);
  const [guesses, setGuesses] = useState<GuessItem[]>([]);
  const [isWon, setIsWon] = useState(false);
  const [loading, setLoading] = useState(false);
  
  // Fake PlayerId for demo (in a real app, use Context or AsyncStore)
  const defaultPlayerId = '00000000-0000-0000-0000-000000000001'; 

  useEffect(() => {
    const initGame = async () => {
      try {
        setLoading(true);
        // Mode 0 = Solo
        const res = await createRoom(0, defaultPlayerId);
        if (res && res.id) {
          setRoomId(res.id);
        } else if (res && res.roomCode) {
          setRoomId(res.roomCode);
        }
      } catch (err: any) {
        console.error('Otaq yaradılarkən xəta:', err);
        Alert.alert('Xəta', 'Oyun başladılarkən xəta: ' + err.message);
      } finally {
        setLoading(false);
      }
    };
    initGame();
  }, []);

  const handleGuess = async (word: string) => {
    if (isWon || !roomId) return;
    setLoading(true);

    try {
      const data = await safeFetch(`${API_URL}/Game/guess`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({ roomId: roomId, playerId: defaultPlayerId, word })
      });
      
      const newItem: GuessItem = {
        id: data.word,
        word: data.word,
        rank: data.rank,
        colorCategory: data.colorCategory,
        isCorrect: data.isCorrect,
        lieMessage: data.lieMessage
      };
      
      setGuesses(prev => [newItem, ...prev]);
      
      if (data.isCorrect) {
        setIsWon(true);
        Alert.alert('Təbriklər! 🎉', 'Sözü tapdınız: ' + data.word.toUpperCase());
      }
    } catch (err: any) {
      console.error(err);
      Alert.alert('Xəta', err.message || 'Şəbəkə xətası.');
    } finally {
      setLoading(false);
    }
  };

  const bestGuess = guesses.length > 0 ? guesses.reduce((prev, curr) => prev.rank < curr.rank ? prev : curr) : null;

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.statsText}>Cəhd: {guesses.length}</Text>
        {bestGuess && (
           <Text style={styles.statsText}>Ən yaxşı rank: {bestGuess.rank}</Text>
        )}
      </View>
      
      {bestGuess && (
        <ColorBar rank={bestGuess.rank} colorCategory={bestGuess.colorCategory} />
      )}

      {loading && guesses.length === 0 ? (
        <View style={styles.center}>
          <ActivityIndicator size="large" color={COLORS.primary} />
          <Text style={{color: COLORS.text, marginTop: 10}}>Yüklənir...</Text>
        </View>
      ) : (
        <>
          <GuessList guesses={guesses} />
          {!isWon && <GuessInput onSubmit={handleGuess} isLoading={loading} />}
        </>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
  },
  center: {
    flex: 1, 
    justifyContent: 'center', 
    alignItems: 'center'
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingHorizontal: 20,
    paddingTop: 10,
  },
  statsText: {
    color: COLORS.textSecondary,
    fontSize: 14,
    fontWeight: '500',
  }
});
