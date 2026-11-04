import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { UserDto } from '@/types/auth';

export interface AuthState {
  accessToken: string | null;
  user: UserDto | null;
  isAuthenticated: boolean;
  isInitializing: boolean;
  hasHydrated: boolean;

  setAuth: (accessToken: string, user: UserDto) => void;
  setAccessToken: (accessToken: string) => void;
  setUser: (user: UserDto) => void;
  clearAuth: () => void;
  setInitializing: (isInitializing: boolean) => void;
  setHasHydrated: (hasHydrated: boolean) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      user: null,
      isAuthenticated: false,
      isInitializing: true,
      hasHydrated: false,

      setAuth: (accessToken, user) =>
        set({
          accessToken,
          user,
          isAuthenticated: true,
          isInitializing: false,
        }),

      setAccessToken: (accessToken) =>
        set({
          accessToken,
          isAuthenticated: Boolean(accessToken),
        }),

      setUser: (user) =>
        set({
          user,
        }),

      clearAuth: () =>
        set({
          accessToken: null,
          user: null,
          isAuthenticated: false,
          isInitializing: false,
        }),

      setInitializing: (isInitializing) =>
        set({
          isInitializing,
        }),

      setHasHydrated: (hasHydrated) =>
        set({
          hasHydrated,
        }),
    }),
    {
      name: 'social_auth_session',
      partialize: (state) => ({
        accessToken: state.accessToken,
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
      onRehydrateStorage: () => (state) => {
        state?.setHasHydrated(true);
      },
    }
  )
);
