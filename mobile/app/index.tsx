import React, { useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, TextInput } from 'react-native';
import { router } from 'expo-router';
import { COLORS, SIZES } from '../constants/theme';
import { SafeAreaView } from 'react-native-safe-area-context';
import { registerPlayer } from '../services/api';

export default function Home() {
  const [username, setUsername] = useState('');
  const [isRegistered, setIsRegistered] = useState(false);

  const handleRegister = async () => {
    if (username.trim().length > 2) {
      try {
        await registerPlayer(username.trim());
        setIsRegistered(true);
      } catch (err) {
        console.error('Registration error', err);
      }
    }
  };

  if (!isRegistered) {
    return (
      <SafeAreaView style={styles.container}>
        <View style={styles.header}>
            <Text style={styles.title}>Semanti<Text style={{color: COLORS.primary}}>X</Text></Text>
            <Text style={styles.subtitle}>AI ilə Semantik Söz Oyunu</Text>
        </View>

        <View style={styles.loginContainer}>
          <Text style={styles.label}>Oyunçu adı:</Text>
          <TextInput
            style={styles.input}
            placeholder="Məsələn: Orxan"
            placeholderTextColor={COLORS.textSecondary}
            value={username}
            onChangeText={setUsername}
            autoCapitalize="words"
          />
          <TouchableOpacity 
            style={[styles.button, !username.trim() && styles.buttonDisabled]}
            onPress={handleRegister}
            disabled={!username.trim()}
          >
            <Text style={styles.buttonText}>Başla</Text>
          </TouchableOpacity>
        </View>
      </SafeAreaView>
    );
  }

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>Semanti<Text style={{color: COLORS.primary}}>X</Text></Text>
        <Text style={styles.subtitle}>Salam, {username}!</Text>
      </View>

      <View style={styles.menuContainer}>
        <TouchableOpacity 
            style={styles.menuItem}
            onPress={() => router.push('/game/solo')}
        >
          <Text style={styles.menuTitle}>Solo Rejim</Text>
          <Text style={styles.menuDesc}>Təkbaşına oyna və düşünmə balını yoxla</Text>
        </TouchableOpacity>

        <TouchableOpacity 
            style={[styles.menuItem, { borderColor: COLORS.error }]}
            onPress={() => router.push('/game/battle')}
        >
          <Text style={styles.menuTitle}>Battle (1v1)</Text>
          <Text style={styles.menuDesc}>Dostunla və ya rəqiblə yarış</Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.menuItem}>
          <Text style={styles.menuTitle}>Speedrun (60s)</Text>
          <Text style={styles.menuDesc}>60 saniyədə ən yaxın sözü tap</Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.menuItem}>
          <Text style={styles.menuTitle}>Multi-Target</Text>
          <Text style={styles.menuDesc}>3-4 əlaqəli sözü tap</Text>
        </TouchableOpacity>

        <TouchableOpacity style={[styles.menuItem, { borderColor: COLORS.warning }]} >
          <Text style={[styles.menuTitle, {color: COLORS.warning}]}>Lie Mode (Sarkastik AI)</Text>
          <Text style={styles.menuDesc}>AI 30% ehtimalla sənə yalan deyəcək!</Text>
        </TouchableOpacity>
      </View>

      <TouchableOpacity 
        style={styles.leaderboardBtn}
        onPress={() => router.push('/leaderboard')}
      >
        <Text style={styles.leaderboardText}>🏆 Liderlər Cədvəli</Text>
      </TouchableOpacity>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
    padding: 20,
  },
  header: {
    alignItems: 'center',
    marginTop: 40,
    marginBottom: 40,
  },
  title: {
    fontSize: 42,
    fontWeight: '900',
    color: COLORS.text,
    letterSpacing: 2,
  },
  subtitle: {
    fontSize: 16,
    color: COLORS.textSecondary,
    marginTop: 8,
  },
  loginContainer: {
    backgroundColor: COLORS.surface,
    padding: 24,
    borderRadius: SIZES.radius,
    borderWidth: 1,
    borderColor: COLORS.border,
  },
  label: {
    color: COLORS.text,
    fontSize: 16,
    marginBottom: 8,
  },
  input: {
    height: 50,
    backgroundColor: COLORS.background,
    borderRadius: SIZES.radius,
    paddingHorizontal: 16,
    color: COLORS.text,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: 16,
    fontSize: 16,
  },
  button: {
    height: 50,
    backgroundColor: COLORS.primary,
    borderRadius: SIZES.radius,
    justifyContent: 'center',
    alignItems: 'center',
  },
  buttonDisabled: {
    opacity: 0.5,
  },
  buttonText: {
    color: COLORS.background,
    fontSize: 18,
    fontWeight: 'bold',
  },
  menuContainer: {
    flex: 1,
  },
  menuItem: {
    backgroundColor: COLORS.surface,
    padding: 20,
    borderRadius: SIZES.radius,
    marginBottom: 16,
    borderWidth: 1,
    borderColor: COLORS.border,
  },
  menuTitle: {
    color: '#FFF',
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  menuDesc: {
    color: COLORS.textSecondary,
    fontSize: 14,
  },
  leaderboardBtn: {
    padding: 20,
    alignItems: 'center',
  },
  leaderboardText: {
    color: COLORS.primary,
    fontSize: 16,
    fontWeight: '600',
  }
});
