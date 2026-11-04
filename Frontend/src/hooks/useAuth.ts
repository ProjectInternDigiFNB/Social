import { useAuthStore } from '@/store/authStore';
import { authApi } from '@/api/authApi';
import { useNavigate } from 'react-router-dom';
import { ROUTES } from '@/constants/routes';
import { authSync } from '@/utils/authSync';

/**
 * Custom hook quản lý trạng thái xác thực và các hành động Auth
 */
export function useAuth() {
  const store = useAuthStore();
  const navigate = useNavigate();

  const logout = async () => {
    try {
      await authApi.logout();
    } catch (err) {
      console.error('Logout error:', err);
    } finally {
      store.clearAuth();
      authSync.broadcast({ type: 'AUTH_LOGOUT' });
      navigate(ROUTES.AUTH.LOGIN);
    }
  };

  return {
    ...store,
    logout,
  };
}
