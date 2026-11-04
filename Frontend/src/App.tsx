import React, { useEffect } from 'react';
import axios from 'axios';
import { BrowserRouter } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { authApi } from '@/api/authApi';
import { profileApi } from '@/api/profileApi';
import { AppRoutes } from '@/routes/AppRoutes';
import { ErrorBoundary } from '@/components/feedback/ErrorBoundary';
import { signalrService } from '@/services/signalrService';
import { isTokenValid } from '@/utils/token';
import { authSync } from '@/utils/authSync';

// Singleton promise để deduplicate quá trình khôi phục phiên khi F5 hoặc React StrictMode mount 2 lần
let restoreSessionPromise: Promise<void> | null = null;

export const App: React.FC = () => {
  const { setAuth, clearAuth, setInitializing, isInitializing, accessToken } = useAuthStore();

  useEffect(() => {
    let isMounted = true;

    // Lắng nghe sự kiện đồng bộ đăng nhập/đăng xuất từ các tab khác
    const unsubscribeSync = authSync.subscribe((event) => {
      if (event.type === 'AUTH_TOKEN_REFRESHED') {
        useAuthStore.getState().setAccessToken(event.accessToken);
      } else if (event.type === 'AUTH_LOGOUT') {
        useAuthStore.getState().clearAuth();
      }
    });

    const initSession = async () => {
      const currentToken = useAuthStore.getState().accessToken;

      // 1. Nếu token đã có sẵn trong store và vẫn còn hạn sử dụng (JWT exp >= 30s)
      // -> Bỏ qua toàn bộ refresh và profile/me! Mở khóa khởi tạo ngay tức thì (0ms).
      if (currentToken && isTokenValid(currentToken)) {
        if (isMounted) {
          setInitializing(false);
        }
        return;
      }

      // 2. Nếu chưa có token hoặc token đã hết hạn -> Thử khôi phục qua HttpOnly Cookie
      if (!restoreSessionPromise) {
        restoreSessionPromise = (async () => {
          try {
            const refreshData = await authApi.refresh();
            if (refreshData?.accessToken) {
              useAuthStore.getState().setAccessToken(refreshData.accessToken);
              const profile = await profileApi.getMyProfile();
              if (isMounted) {
                setAuth(refreshData.accessToken, {
                  id: profile.id,
                  email: profile.email,
                  displayName: profile.displayName,
                  bio: profile.bio,
                  avatarUrl: profile.avatarUrl,
                  emailConfirmed: profile.emailConfirmed,
                  role: profile.role,
                  status: profile.status,
                });
              }
            } else if (isMounted) {
              clearAuth();
            }
          } catch (err: unknown) {
            // Nếu request bị hủy do người dùng reload trang tiếp
            if (
              axios.isCancel(err) ||
              (err as { name?: string })?.name === 'CanceledError' ||
              (err as { code?: string })?.code === 'ERR_CANCELED'
            ) {
              return;
            }
            // Không có cookie refresh token hoặc token hết hạn
            if (isMounted) {
              clearAuth();
            }
          } finally {
            if (isMounted) {
              setInitializing(false);
            }
            restoreSessionPromise = null;
          }
        })();
      }

      await restoreSessionPromise;
    };

    initSession();

    return () => {
      isMounted = false;
      unsubscribeSync();
    };
  }, [setAuth, clearAuth, setInitializing]);

  useEffect(() => {
    if (!isInitializing) {
      signalrService.start();
    }
  }, [isInitializing, accessToken]);

  return (
    <ErrorBoundary>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </ErrorBoundary>
  );
};

export default App;
