import React, { useState } from 'react';
import { View, TextInput, TouchableOpacity, StyleSheet, Text, ActivityIndicator } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import { Ionicons } from '@expo/vector-icons'; // Expo provides this

interface Props {
  onSubmit: (word: string) => void;
  isLoading?: boolean;
}

export const GuessInput: React.FC<Props> = ({ onSubmit, isLoading }) => {
  const [word, setWord] = useState('');

  const handleSubmit = () => {
    if (word.trim().length > 0 && !isLoading) {
      onSubmit(word.trim());
      setWord('');
    }
  };

  return (
    <View style={styles.container}>
      <TextInput
        style={styles.input}
        placeholder="Bir söz yazın..."
        placeholderTextColor={COLORS.textSecondary}
        value={word}
        onChangeText={setWord}
        onSubmitEditing={handleSubmit}
        returnKeyType="send"
        editable={!isLoading}
        autoCapitalize="none"
        autoCorrect={false}
      />
      <TouchableOpacity 
        style={[styles.button, (!word.trim() || isLoading) && styles.buttonDisabled]} 
        onPress={handleSubmit}
        disabled={!word.trim() || isLoading}
      >
        {isLoading ? (
          <ActivityIndicator color={COLORS.background} />
        ) : (
          <Ionicons name="send" size={20} color={COLORS.background} />
        )}
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: SIZES.md,
    marginTop: SIZES.sm,
    paddingHorizontal: 20,
  },
  input: {
    flex: 1,
    height: 50,
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radius,
    paddingHorizontal: SIZES.md,
    color: COLORS.text,
    fontSize: 16,
    borderWidth: 1,
    borderColor: COLORS.border,
  },
  button: {
    width: 50,
    height: 50,
    backgroundColor: COLORS.primary,
    borderRadius: SIZES.radius,
    justifyContent: 'center',
    alignItems: 'center',
    marginLeft: SIZES.sm,
  },
  buttonDisabled: {
    opacity: 0.5,
  },
});
