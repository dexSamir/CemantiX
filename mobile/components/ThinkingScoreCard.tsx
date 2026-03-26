import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';

interface Props {
  score: {
    speedFactor: number;
    logicFactor: number;
    consistencyFactor: number;
    overallScore: number;
    grade: string;
  };
}

export const ThinkingScoreCard: React.FC<Props> = ({ score }) => {
  return (
    <View style={styles.card}>
      <Text style={styles.title}>Düşünmə Balı</Text>
      <View style={styles.gradeContainer}>
        <Text style={styles.gradeText}>{score.grade}</Text>
        <Text style={styles.overallText}>{Math.round(score.overallScore)}/100</Text>
      </View>
      <View style={styles.factorsRow}>
        <View style={styles.factorItem}>
            <Text style={styles.factorLabel}>Sürət</Text>
            <Text style={styles.factorValue}>{Math.round(score.speedFactor)}</Text>
        </View>
        <View style={styles.factorItem}>
            <Text style={styles.factorLabel}>Məntiq</Text>
            <Text style={styles.factorValue}>{Math.round(score.logicFactor)}</Text>
        </View>
        <View style={styles.factorItem}>
            <Text style={styles.factorLabel}>Ardıcıllıq</Text>
            <Text style={styles.factorValue}>{Math.round(score.consistencyFactor)}</Text>
        </View>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  card: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radius,
    padding: SIZES.md,
    marginVertical: SIZES.sm,
    borderWidth: 1,
    borderColor: COLORS.border,
  },
  title: {
    color: COLORS.textSecondary,
    fontSize: 14,
    textTransform: 'uppercase',
    marginBottom: SIZES.sm,
  },
  gradeContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SIZES.md,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.border,
    paddingBottom: SIZES.sm,
  },
  gradeText: {
    color: COLORS.text,
    fontSize: 24,
    fontWeight: 'bold',
  },
  overallText: {
    color: COLORS.primary,
    fontSize: 20,
    fontWeight: 'bold',
  },
  factorsRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  factorItem: {
    alignItems: 'center',
  },
  factorLabel: {
    color: COLORS.textSecondary,
    fontSize: 12,
    marginBottom: 4,
  },
  factorValue: {
    color: '#FFF',
    fontSize: 16,
    fontWeight: '600',
  },
});
