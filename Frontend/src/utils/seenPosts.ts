const STORAGE_KEY = 'social_seen_post_ids';
const MAX_SEEN_IDS = 300;
const FLUSH_DELAY_MS = 600; // Batching: Gom các lần ghi localStorage lại sau 600ms

let memoryCache: string[] | null = null;
let pendingTimer: ReturnType<typeof setTimeout> | null = null;
let isDirty = false;

function loadFromStorage(): string[] {
  try {
    const data = localStorage.getItem(STORAGE_KEY);
    if (!data) return [];
    const parsed = JSON.parse(data);
    return Array.isArray(parsed) ? parsed : [];
  } catch {
    return [];
  }
}

function flushToStorage(): void {
  if (!isDirty || !memoryCache) return;
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(memoryCache));
    isDirty = false;
  } catch (e) {
    console.warn('[SeenPostsService] Failed to flush seen post ids', e);
  }
}

// Tự động flush khi người dùng đóng trang hoặc chuyển tab
if (typeof window !== 'undefined') {
  window.addEventListener('beforeunload', () => {
    if (pendingTimer) clearTimeout(pendingTimer);
    flushToStorage();
  });
  document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'hidden') {
      if (pendingTimer) clearTimeout(pendingTimer);
      flushToStorage();
    }
  });
}

function scheduleFlush(): void {
  isDirty = true;
  if (pendingTimer) clearTimeout(pendingTimer);
  pendingTimer = setTimeout(() => {
    pendingTimer = null;
    flushToStorage();
  }, FLUSH_DELAY_MS);
}

export const seenPostsService = {
  /**
   * Lấy toàn bộ danh sách ID các bài viết đã xem (ưu tiên đọc từ memory cache O(1))
   */
  getSeenIds(): string[] {
    if (memoryCache === null) {
      memoryCache = loadFromStorage();
    }
    return memoryCache;
  },

  /**
   * Lấy danh sách ID đã xem gần nhất (giới hạn số lượng) để gửi lên API
   */
  getRecentSeenIds(limit: number = 60): string[] {
    const ids = this.getSeenIds();
    return ids.slice(0, limit);
  },

  /**
   * Đánh dấu một bài viết là đã xem (Non-blocking, batch ghi sau)
   */
  markAsSeen(id: string): void {
    if (!id) return;
    const current = this.getSeenIds();
    if (current[0] === id) return; // Đã là bài gần nhất, không cần xử lý

    const filtered = current.filter((item) => item !== id);
    filtered.unshift(id);
    if (filtered.length > MAX_SEEN_IDS) {
      filtered.length = MAX_SEEN_IDS;
    }
    memoryCache = filtered;
    scheduleFlush();
  },

  /**
   * Đánh dấu hàng loạt bài viết là đã xem
   */
  markMultipleAsSeen(ids: string[]): void {
    if (!ids || ids.length === 0) return;
    const current = this.getSeenIds();
    const set = new Set(ids);
    const remaining = current.filter((item) => !set.has(item));
    const combined = [...ids, ...remaining];
    if (combined.length > MAX_SEEN_IDS) {
      combined.length = MAX_SEEN_IDS;
    }
    memoryCache = combined;
    scheduleFlush();
  },

  /**
   * Kiểm tra bài viết đã xem hay chưa
   */
  hasSeen(id: string): boolean {
    return this.getSeenIds().includes(id);
  },

  /**
   * Xóa lịch sử bài đã xem
   */
  clear(): void {
    memoryCache = [];
    isDirty = false;
    if (pendingTimer) clearTimeout(pendingTimer);
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch (e) {
      console.warn('[SeenPostsService] Failed to clear seen posts', e);
    }
  },
};
