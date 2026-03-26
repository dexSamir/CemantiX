import React, { useEffect, useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';

interface Props {
  initialSeconds: number;
  onTimeUp: () => void;
  isRunning: boolean;
}

export const Timer: React.FC<Props> = ({ initialSeconds, onTimeUp, isRunning }) => {
  const [secondsLeft, setSecondsLeft] = useState(initialSeconds);

  useEffect(() => {
    let interval: NodeJS.Timeout;

    if (isRunning && secondsLeft > 0) {
      interval = setInterval(() => {
        setSecondsLeft((prev) => {
          if (prev <= 1) {
            clearInterval(interval);
            onTimeUp();
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }

    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isRunning, secondsLeft, onTimeUp]);

  const progressStyle = {
    width: `${(secondsLeft / initialSeconds) * 100}%`,
    backgroundColor: secondsLeft <= 10 ? COLORS.error : COLORS.primary,
  };

  return (
    <View style={styles.container}>
      <Text style={styles.text}>{secondsLeft}s</Text>
      <View style={styles.progressBarBg}>
        <View style={[styles.progressBarFill, progressStyle]} />
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: 20,
    marginVertical: 10,
    alignItems: 'center',
    width: '100%',
  },
  text: {
    color: '#FFF',
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: SIZES.sm,
  },
  progressBarBg: {
    width: '100%',
    height: 8,
    backgroundColor: COLORS.surface,
    borderRadius: 4,
    overflow: 'hidden',
  },
  progressBarFill: {
    height: '100%',
    borderRadius: 4,
  },
});
