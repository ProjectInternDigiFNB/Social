export type AuthSyncEvent =
  | { type: 'AUTH_TOKEN_REFRESHED'; accessToken: string }
  | { type: 'AUTH_LOGOUT' };

const CHANNEL_NAME = 'social_auth_sync_channel';

class AuthSyncManager {
  private channel: BroadcastChannel | null = null;
  private listeners = new Set<(event: AuthSyncEvent) => void>();

  constructor() {
    if (typeof window !== 'undefined' && 'BroadcastChannel' in window) {
      try {
        this.channel = new BroadcastChannel(CHANNEL_NAME);
        this.channel.onmessage = (messageEvent) => {
          if (messageEvent?.data && typeof messageEvent.data === 'object') {
            this.notifyListeners(messageEvent.data as AuthSyncEvent);
          }
        };
      } catch (err) {
        console.warn('[AuthSync] Không thể khởi tạo BroadcastChannel:', err);
      }
    }
  }

  public broadcast(event: AuthSyncEvent): void {
    if (this.channel) {
      try {
        this.channel.postMessage(event);
      } catch (err) {
        console.warn('[AuthSync] Lỗi khi gửi broadcast event:', err);
      }
    }
  }

  public subscribe(listener: (event: AuthSyncEvent) => void): () => void {
    this.listeners.add(listener);
    return () => {
      this.listeners.delete(listener);
    };
  }

  private notifyListeners(event: AuthSyncEvent): void {
    this.listeners.forEach((listener) => {
      try {
        listener(event);
      } catch (err) {
        console.error('[AuthSync] Lỗi xử lý listener:', err);
      }
    });
  }
}

export const authSync = new AuthSyncManager();
