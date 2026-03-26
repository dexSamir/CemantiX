import React from 'react';
import { View, Text, StyleSheet, FlatList } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import { getColorForCategory } from '../utils/colorScale';

export interface GuessItem {
  id: string; // or purely index
  word: string;
  rank: number;
  colorCategory: string;
  isCorrect: boolean;
  hint?: string;
  lieMessage?: string;
}

interface Props {
  guesses: GuessItem[];
}

export const GuessList: React.FC<Props> = ({ guesses }) => {
  const renderItem = ({ item, index }: { item: GuessItem, index: number }) => {
    const color = getColorForCategory(item.colorCategory);
    
    return (
        <View style={styles.guessItemContainer}>
            <View style={[styles.guessItem, { borderColor: item.isCorrect ? COLORS.primary : COLORS.border }]}>
                <View style={styles.leftContent}>
                    <Text style={styles.number}>#{guesses.length - index}</Text>
                    <Text style={styles.word}>{item.word}</Text>
                </View>
                <View style={[styles.rankBadge, { backgroundColor: color + '20' }]}>
                    <Text style={[styles.rankValue, { color }]}>{item.rank}</Text>
                </View>
            </View>
            
            {/* Contexto Multi-Target Hint or Lie Mode Sarcasm */}
            {item.hint && (
                <Text style={styles.hintText}>{item.hint}</Text>
            )}
            {item.lieMessage && (
                <Text style={styles.lieText}>{item.lieMessage}</Text>
            )}
        </View>
    );
  };

  return (
    <FlatList
      data={guesses}
      keyExtractor={(item, index) => `${item.word}-${index}`}
      renderItem={renderItem}
      contentContainerStyle={styles.listContainer}
      showsVerticalScrollIndicator={false}
    />
  );
};

const styles = StyleSheet.create({
  listContainer: {
    paddingHorizontal: 20,
    paddingBottom: 20,
  },
  guessItemContainer: {
    marginBottom: SIZES.sm,
  },
  guessItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radius,
    padding: 12,
    borderWidth: 1,
  },
  leftContent: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  number: {
    color: COLORS.textSecondary,
    fontSize: 12,
    marginRight: 10,
    width: 30,
  },
  word: {
    color: COLORS.text,
    fontSize: 16,
    fontWeight: '500',
  },
  rankBadge: {
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 12,
  },
  rankValue: {
    fontWeight: 'bold',
    fontSize: 14,
  },
  hintText: {
    color: COLORS.info,
    fontSize: 12,
    marginTop: 4,
    marginLeft: 10,
    fontStyle: 'italic',
  },
  lieText: {
    color: COLORS.warning,
    fontSize: 12,
    marginTop: 4,
    marginLeft: 10,
    fontStyle: 'italic',
  }
});
