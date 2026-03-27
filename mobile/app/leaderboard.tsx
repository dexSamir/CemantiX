import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Text, ActivityIndicator, FlatList } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import { API_URL, safeFetch } from '../services/api';

interface LeaderboardEntry {
  rank: number;
  playerId: string;
  username: string;
  score: number;
  attemptCount: number;
}

export default function Leaderboard() {
  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchLeaderboard = async () => {
      try {
        const data = await safeFetch(`${API_URL}/Leaderboard/Solo/alltime`);
        setEntries(data);
      } catch (err: any) {
        console.error('Liderlər tablosu yüklənərkən xəta:', err);
      } finally {
        setLoading(false);
      }
    };
    fetchLeaderboard();
  }, []);

  const renderItem = ({ item }: { item: LeaderboardEntry }) => (
    <View style={styles.item}>
      <Text style={styles.rankBadge}>#{item.rank}</Text>
      <View style={styles.info}>
        <Text style={styles.username}>{item.username}</Text>
        <Text style={styles.details}>Cəhd: {item.attemptCount}</Text>
      </View>
      <Text style={styles.score}>{item.score}</Text>
    </View>
  );

  return (
    <View style={styles.container}>
      <Text style={styles.title}>All-Time Liderlər (Solo)</Text>
      
      {loading ? (
        <ActivityIndicator size="large" color={COLORS.primary} />
      ) : (
        <FlatList
          data={entries}
          keyExtractor={(e) => e.playerId}
          renderItem={renderItem}
          contentContainerStyle={{ paddingBottom: 20 }}
          ItemSeparatorComponent={() => <View style={{ height: 10 }} />}
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#FFF',
    marginBottom: 20,
    textAlign: 'center',
  },
  item: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surface,
    padding: SIZES.md,
    borderRadius: SIZES.radius,
  },
  rankBadge: {
    color: COLORS.primary,
    fontSize: 20,
    fontWeight: 'bold',
    width: 40,
  },
  info: {
    flex: 1,
  },
  username: {
    color: '#FFF',
    fontSize: 18,
    fontWeight: 'bold',
  },
  details: {
    color: COLORS.textSecondary,
    fontSize: 12,
  },
  score: {
    color: COLORS.warning,
    fontSize: 22,
    fontWeight: 'bold',
  }
});
