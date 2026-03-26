import { DarkTheme, ThemeProvider } from '@react-navigation/native';
import { Stack } from 'expo-router';
import { COLORS } from '../constants/theme';
import { StatusBar } from 'expo-status-bar';

export default function RootLayout() {
  return (
    <ThemeProvider value={{
      ...DarkTheme,
      colors: {
        ...DarkTheme.colors,
        background: COLORS.background,
        card: COLORS.surface,
        border: COLORS.border,
        text: COLORS.text,
        primary: COLORS.primary,
      }
    }}>
      <StatusBar style="light" />
      <Stack
        screenOptions={{
          headerStyle: { backgroundColor: COLORS.surface },
          headerTintColor: COLORS.text,
          headerShadowVisible: false,
        }}
      >
        <Stack.Screen name="index" options={{ title: 'SemantiX', headerShown: false }} />
        <Stack.Screen name="game/solo" options={{ title: 'Solo Oyun', presentation: 'fullScreenModal' }} />
        <Stack.Screen name="game/battle" options={{ title: 'Otaq Battle', presentation: 'fullScreenModal' }} />
        <Stack.Screen name="leaderboard" options={{ title: 'Liderlər', presentation: 'modal' }} />
      </Stack>
    </ThemeProvider>
  );
}
