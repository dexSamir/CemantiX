import React from 'react';
import { View, StyleSheet, Text, Dimensions } from 'react-native';
import { COLORS } from '../constants/theme';
import { getColorForCategory } from '../utils/colorScale';

interface Props {
  rank: number;
  colorCategory: string; // 'hot', 'warm', 'cool', 'cold'
}

export const ColorBar: React.FC<Props> = ({ rank, colorCategory }) => {
  // We want a bar that extends up to a certain point based on rank.
  // Rank 1 = 100% width, Rank 10000 = ~5% width.
  const maxWidth = Dimensions.get('window').width - 40; // 20px padding on each side
  const fillPercentage = Math.max(0.05, 1 - (Math.min(rank, 10000) / 10000));
  const fillWidth = maxWidth * fillPercentage;
  
  const barColor = getColorForCategory(colorCategory);

  return (
    <View style={styles.container}>
      <View style={[styles.barBackground, { width: maxWidth }]}>
        <View style={[styles.barFill, { width: fillWidth, backgroundColor: barColor }]} />
      </View>
      <View style={styles.textContainer}>
        <Text style={[styles.rankText, { color: barColor }]}>Rank: {rank}</Text>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginVertical: 10,
    alignItems: 'center',
    width: '100%',
  },
  barBackground: {
    height: 12,
    backgroundColor: COLORS.surface,
    borderRadius: 6,
    overflow: 'hidden',
  },
  barFill: {
    height: '100%',
    borderRadius: 6,
  },
  textContainer: {
    marginTop: 4,
    alignSelf: 'flex-end',
    paddingRight: 20,
  },
  rankText: {
    fontWeight: 'bold',
    fontSize: 14,
  },
});
