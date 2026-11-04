import axios from 'axios';
import { useAuthStore } from '@/store/authStore';
import { ENV } from '@/config';
import { getApiErrorMessage } from '@/utils/error';
import { seenPostsService } from '@/utils/seenPosts';
import { authSync } from '@/utils/authSync';

export { getApiErrorMessage };

export const apiClient = axios.create({
  baseURL: ENV.API_BASE_URL,
  timeout: 60000,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

// Request interceptor: Gắn Access Token từ Zustand store và danh sách bài đã xem (Non-blocking)
apiClient.interceptors.request.use(
  (config) => {
    const accessToken = useAuthStore.getState().accessToken;
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }

    // Tự động gắn danh sách bài viết đã xem gần đây vào header đối với các request lấy bảng tin
    if (config.method === 'get' && config.url && config.url.includes('/posts')) {
      const recentSeen = seenPostsService.getRecentSeenIds(60);
      if (recentSeen.length > 0) {
        config.headers['X-Seen-Post-Ids'] = recentSeen.join(',');
      }
    }

    return config;
  },
  (error) => {
    console.error('[API Request Error]', error);
    return Promise.reject(error);
  }
);

let isRefreshing = false;
let failedQueue: Array<{
  resolve: (token: string | null) => void;
  reject: (error: unknown) => void;
}> = [];

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// Response interceptor: Single-flight Reactive Refresh bắt mã 401
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Không thử refresh nếu là các API auth cơ bản hoặc request đã từng retry
    const isAuthRoute =
      originalRequest?.url?.includes('/auth/login') ||
      originalRequest?.url?.includes('/auth/register') ||
      originalRequest?.url?.includes('/auth/refresh') ||
      originalRequest?.url?.includes('/auth/forgot-password') ||
      originalRequest?.url?.includes('/auth/reset-password');

    if (error.response?.status === 401 && originalRequest && !originalRequest._retry && !isAuthRoute) {
      // Nếu đã có 1 request refresh đang chạy trong tab, xếp vào hàng đợi chờ kết quả
      if (isRefreshing) {
        return new Promise<string | null>((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          if (token) {
            originalRequest.headers.Authorization = `Bearer ${token}`;
          }
          return apiClient(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        // Refresh token được trình duyệt tự gửi qua cookie httpOnly
        const refreshResponse = await axios.post<{ accessToken: string; expiresAt: string }>(
          `${ENV.API_BASE_URL}/auth/refresh`,
          {},
          { withCredentials: true }
        );

        const newAccessToken = refreshResponse.data.accessToken;
        useAuthStore.getState().setAccessToken(newAccessToken);

        // Đồng bộ token mới sang các tab khác đang mở
        authSync.broadcast({
          type: 'AUTH_TOKEN_REFRESHED',
          accessToken: newAccessToken,
        });

        processQueue(null, newAccessToken);
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return apiClient(originalRequest);
      } catch (refreshErr) {
        processQueue(refreshErr, null);
        useAuthStore.getState().clearAuth();

        // Đồng bộ sự kiện đăng xuất / hết phiên sang các tab khác
        authSync.broadcast({ type: 'AUTH_LOGOUT' });

        return Promise.reject(refreshErr);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);
