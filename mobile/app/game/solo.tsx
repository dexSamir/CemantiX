import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Text, ActivityIndicator, Alert } from 'react-native';
import { COLORS } from '../../constants/theme';
import { GuessInput } from '../../components/GuessInput';
import { GuessList, GuessItem } from '../../components/GuessList';
import { ColorBar } from '../../components/ColorBar';
import { API_URL } from '../../services/api';

export default function SoloGame() {
  const [roomId, setRoomId] = useState<string | null>(null);
  const [guesses, setGuesses] = useState<GuessItem[]>([]);
  const [isWon, setIsWon] = useState(false);
  const [loading, setLoading] = useState(false);
  
  // Fake PlayerId for demo (in a real app, use Context or AsyncStore)
  const defaultPlayerId = '00000000-0000-0000-0000-000000000001'; 

  const handleGuess = async (word: string) => {
    if (isWon) return;
    setLoading(true);

    try {
      // For solo play without room creation overhead for now, wait for game backend enhancements 
      // where we just submit to /similarity directly, but since we designed it room-based
      // we'll assume the room is already created in a full implementation, or we mock it.
      // For this demo structure, we use a mocked response if room doesn't exist yet,
      // but ideally we'd hit the API.
      
      const res = await fetch(`${API_URL}/Game/guess`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        // Room ID is hardcoded for simple solo mock, must be dynamic in real build
        body: JSON.stringify({ roomId: defaultPlayerId, playerId: defaultPlayerId, word })
      });
      
      const data = await res.json();
      
      if (res.ok) {
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
      } else {
        // Fallback or error handling
        Alert.alert('Xəta', data.error || 'Söz yoxlanılarkən xəta baş verdi');
      }
    } catch (err) {
      console.error(err);
      Alert.alert('Xəta', 'Şəbəkə xətası.');
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
